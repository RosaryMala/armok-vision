using UnityEngine;
using System.Collections;
using DFHack;
using System.Collections.Generic;
using RemoteFortressReader;

[System.Flags]
public enum Directions
{
    None = 0,
    NorthWest = 1,
    North = 2,
    NorthEast = 4,
    East = 8,
    SouthEast = 16,
    South = 32,
    SouthWest = 64,
    West = 128,
    NorthWestCorner = North | West | NorthWest,
    NorthEastCorner = North | East | NorthEast,
    SouthEastCorner = South | East | SouthEast,
    SouthWestCorner = South | West | SouthWest
}

// A store for a section of map tiles.
// One large instance is used as main storage for the game map;
// Smaller slices are used temporary storage for meshing.

// Read tiles with store[DFCoord] or store[int, int, int], using world coordinates. You'll get a Tile?.
// Modify tiles like so: store.InitOrModifyTile(DFCoord, tiletype: 4, water_level: 2), and so on.
// (It's deliberately impossible to wholly overwrite tiles, so that we can't accidentally set tiles
// to have incorrect positions or containers.)

public class MapDataStore {

    // Static stuff

    // The "main" map, used in the main Unity thread.
    static MapDataStore _main;
    public static MapDataStore Main {
        get {
            // TODO throw exception if accessed from non-main-thread.
            return _main;
        }
        private set {
            if (_main != null && value != null) throw new UnityException("Main Map already initialized!");
            _main = value;
        }
    }
    // Called from DFConnection.InitStatics
    public static void InitMainMap(int xSize, int ySize, int zSize) {
        MapSize = new DFCoord(xSize, ySize, zSize);
        _main = new MapDataStore(new DFCoord(0,0,0), MapSize);
        Debug.Log("Set main map to " + MapSize);
    }

    // The size of the whole map.
    public static DFCoord MapSize { get; private set; }
    public static bool InMapBounds (DFCoord coord) {
        return 0 <= coord.x && coord.x < MapSize.x &&
                0 <= coord.y && coord.y < MapSize.y &&
                0 <= coord.z && coord.z < MapSize.z;
    }

    // Used for dynamic material loading and stuff.
    public static List<Tiletype> tiletypeTokenList { private get; set; }
    // Used to index into water/magma arrays in a few places.
    public const int WATER_INDEX = 0;
    public const int MAGMA_INDEX = 1;

    public static readonly DFCoord BLOCK_SIZE = new DFCoord(16,16,1);


    // Instance stuff

    // The size of this slice of the map
    public readonly DFCoord SliceSize;
    // The origin of this slice of the map; subtracted from indices when indexed
    // (So that code using slices of the map can use the same coordinates as when
    // accessing the main map)
    public DFCoord SliceOrigin { get; private set; }
    // The data
    Tile[,,] tiles;
    // An array for whether a tile is present or not; index with PresentIndex
    // (We use this since it's cheaper than storing an array of Tile?s)
    BitArray tilesPresent;

    private MapDataStore() {}

    public MapDataStore(DFCoord origin, DFCoord sliceSize) {
        if (sliceSize.x < 0 || sliceSize.x > MapSize.x || 
            sliceSize.y < 0 || sliceSize.y > MapSize.y || 
            sliceSize.z < 0 || sliceSize.z > MapSize.z) {
            throw new UnityException("Can't have a map slice outside the map!");
        }
        if (sliceSize.x < 1 || sliceSize.y < 1 || sliceSize.z < 1) {
            Debug.Log("Can't make map slice without any tiles, making a minimal sized one instead.");
            sliceSize.x = 16;
            sliceSize.y = 16;
            sliceSize.z = 1;
        }
        SliceSize = sliceSize;
        tiles = new Tile[SliceSize.x, SliceSize.y, SliceSize.z];
        tilesPresent = new BitArray(PresentIndex(SliceSize.x-1, SliceSize.y-1, SliceSize.z-1)+1);
        Reset();
    }

    public MapDataStore(BlockCoord block) : this(block.ToDFCoord(), BLOCK_SIZE){}

    public void CopySliceTo(DFCoord newSliceOrigin, DFCoord newSliceSize, MapDataStore target) {
        if (newSliceSize != target.SliceSize) {
            throw new UnityException("Mismatched slice sizes");
        }
        var localNewSliceOrigin = WorldToLocalSpace(newSliceOrigin);
        if (!InSliceBoundsLocal(localNewSliceOrigin) || !InSliceBoundsLocal(localNewSliceOrigin + newSliceSize - new DFCoord(1,1,1))) {
            throw new UnityException("Can't slice outside of our slice bounds");
        }
        for (int x = 0; x < newSliceSize.x; x++) {
            for (int y = 0; y < newSliceSize.y; y++) {
                for (int z = 0; z < newSliceSize.z; z++) {
                    //pre-calculate it before we copy, because afterwards we won't have contextual data.
                    if (tiles[localNewSliceOrigin.x + x, localNewSliceOrigin.y + y, localNewSliceOrigin.z + z].shape == TiletypeShape.RAMP)
                        tiles[localNewSliceOrigin.x + x, localNewSliceOrigin.y + y, localNewSliceOrigin.z + z].CalculateRampType(); 
                    target.tiles[x, y, z] = tiles[localNewSliceOrigin.x+x, localNewSliceOrigin.y+y, localNewSliceOrigin.z+z];
                    target.tiles[x, y, z].container = target;
                    target.tilesPresent[target.PresentIndex(x, y, z)] = tilesPresent[PresentIndex(localNewSliceOrigin.x+x, localNewSliceOrigin.y+y, localNewSliceOrigin.z+z)];
                }
            }
        }
        target.SliceOrigin = newSliceOrigin;
    }

    public void CopySliceTo(BlockCoord block, MapDataStore target) {
        CopySliceTo(block.ToDFCoord(), BLOCK_SIZE, target);
    }

    public MapDataStore CopySlice(DFCoord newSliceOrigin, DFCoord newSliceSize) {
        MapDataStore target = new MapDataStore(newSliceOrigin, newSliceSize);
        CopySliceTo(newSliceOrigin, newSliceSize, target);
        return target;
    }

    public MapDataStore CopySlice(BlockCoord block) {
        return CopySlice(block.ToDFCoord(), BLOCK_SIZE);
    }

    public void StoreTiles(RemoteFortressReader.MapBlock block) {
        bool setTiles = block.tiles.Count > 0;
        bool setLiquids = block.water.Count > 0 || block.magma.Count > 0;
        if (!setTiles && !setLiquids) return;

        for (int xx = 0; xx < 16; xx++)
            for (int yy = 0; yy < 16; yy++)
            {
                DFCoord worldCoord = new DFCoord(block.map_x + xx, block.map_y + yy, block.map_z);
                DFCoord localCoord = WorldToLocalSpace(worldCoord);
                if (!InSliceBoundsLocal(localCoord))
                {
                    Debug.LogError(worldCoord + " is out of bounds for " + MapSize);
                    return;
                }
                int netIndex = xx + (yy * 16);
                tilesPresent[PresentIndex(localCoord.x, localCoord.y, localCoord.z)] = true;
                if (setTiles) {
                    tiles[localCoord.x, localCoord.y, localCoord.z].tileType = block.tiles[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].material = block.materials[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].base_material = block.base_materials[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].layer_material = block.layer_materials[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].vein_material = block.vein_materials[netIndex];
                    if (block.construction_items != null && block.construction_items.Count > netIndex)
                        tiles[localCoord.x, localCoord.y, localCoord.z].construction_item = block.construction_items[netIndex];
                    else
                        tiles[localCoord.x, localCoord.y, localCoord.z].construction_item = new MatPairStruct(-1, -1);
                }
                if (setLiquids) {
                    tiles[localCoord.x, localCoord.y, localCoord.z].waterLevel = block.water[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].magmaLevel = block.magma[netIndex];
                }
            }
        foreach (var building in block.buildings)
        {
            for (int xx = building.pos_x_min; xx <= building.pos_x_max; xx++)
                for (int yy = building.pos_y_min; yy <= building.pos_y_max; yy++)
                {

                    if((building.building_type.building_type == 29 || building.building_type.building_type == 29)
                        && building.room != null && building.room.extents.Count > 0)
                    {
                        int buildingLocalX = xx - building.room.pos_x;
                        int buildingLocalY = yy - building.room.pos_y;

                        if (building.room.extents[buildingLocalY * building.room.width + buildingLocalX] == 0)
                            continue;
                    }

                    DFCoord worldCoord = new DFCoord(xx,yy, block.map_z);
                    DFCoord localCoord = WorldToLocalSpace(worldCoord);
                    DFCoord2d buildingLocalCoord = new DFCoord2d(xx - building.pos_x_min, yy - building.pos_y_min);
                    if (!InSliceBoundsLocal(localCoord))
                    {
                        Debug.LogError(worldCoord + " is out of bounds for " + MapSize);
                        continue;
                    }
                    tilesPresent[PresentIndex(localCoord.x, localCoord.y, localCoord.z)] = true;
                    tiles[localCoord.x, localCoord.y, localCoord.z].buildingType = building.building_type;
                    tiles[localCoord.x, localCoord.y, localCoord.z].buildingMaterial = building.material;
                    tiles[localCoord.x, localCoord.y, localCoord.z].buildingLocalPos = buildingLocalCoord;
                }
        }
    }

    public bool InSliceBounds(DFCoord loc) {
        return InSliceBoundsLocal(WorldToLocalSpace(loc));
    }

    // Things to read and modify the map
    // Note: everything takes coordinates in world / DF space
    public Tile? this[int x, int y, int z] {
        get {
            return this[new DFCoord(x,y,z)];
        }
    }

    public Tile? this[DFCoord coord] {
        get {
            DFCoord local = WorldToLocalSpace(coord);
            if (InSliceBoundsLocal(local.x, local.y, local.z) && tilesPresent[PresentIndex(local.x, local.y, local.z)]) {
                return tiles[local.x, local.y, local.z];
            } else {
                return null;
            }
        }
    }

    public int GetLiquidLevel(DFCoord coord, int liquidIndex) {
        var tile = this[coord];
        if (tile == null) return 0;
        switch (liquidIndex) {
        case WATER_INDEX:
            return tile.Value.waterLevel;
        case MAGMA_INDEX:
            return tile.Value.magmaLevel;
        default:
            throw new UnityException("No liquid with index "+liquidIndex);
        }
    }

    public void SetLiquidLevel(DFCoord coord, int liquidIndex, int liquidLevel) {
        switch (liquidIndex) {
        case WATER_INDEX:
            InitOrModifyTile(coord, waterLevel: liquidLevel);
            return;
        case MAGMA_INDEX:
            InitOrModifyTile(coord, magmaLevel: liquidLevel);
            return;
        default:
            throw new UnityException("No liquid with index "+liquidIndex);
        }
    }

    public void InitOrModifyTile(DFCoord coord,
                           int? tileType = null,
                           MatPairStruct? material = null,
                           MatPairStruct? base_material = null,
                           MatPairStruct? layer_material = null,
                           MatPairStruct? vein_material = null,
                           int? waterLevel = null,
                           int? magmaLevel = null,
                           MatPairStruct? construction_item = null,
                           int? rampType = null,
                           BuildingStruct? buildingType = null,
                           MatPairStruct? buildingMaterial = null,
                           DFCoord2d? buildingLocalPos = null)
    {
        DFCoord local = WorldToLocalSpace(coord);
        if (!InSliceBoundsLocal(local.x, local.y, local.z)) {
            throw new UnityException("Can't modify tile outside of slice");
        }
        tilesPresent[PresentIndex(local.x, local.y, local.z)] = true;
        tiles[local.x, local.y, local.z].Modify(tileType, material, base_material, layer_material, vein_material, waterLevel, magmaLevel, construction_item, rampType, buildingType, buildingMaterial, buildingLocalPos);
    }

    public void Reset() {
        tilesPresent.SetAll(false);
        for (int x = 0; x < SliceSize.x; x++) {
            for (int y = 0; y < SliceSize.y; y++) {
                for (int z = 0; z < SliceSize.z; z++) {
                    tiles[x,y,z] = new Tile(this, LocalToWorldSpace(new DFCoord(x,y,z)));
                }
            }
        }
    }

    // Helpers
    public DFCoord WorldToLocalSpace(DFCoord coord) {
        return coord - SliceOrigin;
    }
    public DFCoord LocalToWorldSpace(DFCoord coord) {
        return coord + SliceOrigin;
    }
    // These take local space coordinates:
    bool InSliceBoundsLocal(int x, int y, int z) {
        return 0 <= x && x < SliceSize.x &&
                0 <= y && y < SliceSize.y &&
                0 <= z && z < SliceSize.z;
    }
    bool InSliceBoundsLocal(DFCoord coord) {
        return InSliceBoundsLocal(coord.x, coord.y, coord.z);
    }
    int PresentIndex(int x, int y, int z) {
        return x * SliceSize.y * SliceSize.z + y * SliceSize.z + z;
    }

    // The data for a single tile of the map.
    // Nested struct because it depends heavily on its container.
    public struct Tile
    {
        public Tile(MapDataStore container, DFCoord position) {
            this.container = container;
            this.position = position;
            tileType = default(int);
            material = default(MatPairStruct);
            base_material = default(MatPairStruct);
            layer_material = default(MatPairStruct);
            vein_material = default(MatPairStruct);
            construction_item = default(MatPairStruct);
            waterLevel = default(int);
            magmaLevel = default(int);
            rampType = 0;
            buildingMaterial = default(MatPairStruct);
            buildingType = default(BuildingStruct);
            buildingLocalPos = default(DFCoord2d);
        }

        public MapDataStore container;
        public DFCoord position;
        public int tileType;
        public MatPairStruct material;
        public MatPairStruct base_material;
        public MatPairStruct layer_material;
        public MatPairStruct vein_material;
        public MatPairStruct construction_item;
        public int waterLevel;
        public int magmaLevel;
        int rampType;
        public BuildingStruct buildingType;
        public MatPairStruct buildingMaterial;
        public DFCoord2d buildingLocalPos;

        public TiletypeShape shape { get { return tiletypeTokenList [tileType].shape; } }
        public TiletypeMaterial tiletypeMaterial { get { return tiletypeTokenList [tileType].material; } }
        public TiletypeSpecial special { get { return tiletypeTokenList [tileType].special; } }
        public TiletypeVariant variant { get { return tiletypeTokenList [tileType].variant; } }
        public string direction { get { return tiletypeTokenList [tileType].direction; } }

        public int RampType
        {
            get
            {
                if (shape != TiletypeShape.RAMP)
                    return 0;
                if (rampType == 0)
                    CalculateRampType();
                return rampType;
            }
            set
            {
                rampType = value;
            }
        }

        public void Modify (int? tileType = null,
                           MatPairStruct? material = null,
                           MatPairStruct? base_material = null,
                           MatPairStruct? layer_material = null,
                           MatPairStruct? vein_material = null,
                           int? waterLevel = null,
                           int? magmaLevel = null,
                           MatPairStruct? construction_item = null,
                           int? rampType = null,
                           BuildingStruct? buildingType = null,
                           MatPairStruct? buildingMaterial = null,
                           DFCoord2d? buildingLocalPos = null)
        {
            if (tileType != null) {
                this.tileType = tileType.Value;
            }
            if (material != null) {
                this.material = material.Value;
            }
            if (base_material != null) {
                this.base_material = base_material.Value;
            }
            if (layer_material != null) {
                this.layer_material = layer_material.Value;
            }
            if (vein_material != null) {
                this.vein_material = vein_material.Value;
            }
            if (waterLevel != null) {
                this.waterLevel = waterLevel.Value;
            }
            if (magmaLevel != null) {
                this.magmaLevel = magmaLevel.Value;
            }
            if (construction_item != null)
            {
                this.construction_item = construction_item.Value;
            }
            if(rampType != null)
            {
                this.rampType = rampType.Value;
            }
            if(buildingType != null)
            {
                this.buildingType = buildingType.Value;
            }
            if(buildingMaterial != null)
            {
                this.buildingMaterial = buildingMaterial.Value;
            }
            if (buildingLocalPos != null)
                this.buildingLocalPos = buildingLocalPos.Value;
        }
        public bool isWall {
            get {
                switch (shape) {
                case TiletypeShape.WALL:
                case TiletypeShape.FORTIFICATION:
                //case TiletypeShape.BROOK_BED: //Dwarfs can't go through this, but it's permiable to water and also doesn't generally have solid tiles.
                case TiletypeShape.TREE_SHAPE:
                    return true;
                default:
                    return false;
                }
            }
        }
        public bool isWallBuilding
        {
            get
            {
                return isWall
                    || buildingType.building_type == 8 //Door
                    || buildingType.building_type == 9 //Floodgate
                    || buildingType.building_type == 16 //WindowGlass
                    || buildingType.building_type == 17 //WindowGem
                    ;
            }
        }
        public bool isFloor {
            get {
                switch (shape) {
                case TiletypeShape.RAMP:
                case TiletypeShape.FLOOR:
                case TiletypeShape.BOULDER:
                case TiletypeShape.PEBBLES:
                //case TiletypeShape.BROOK_TOP: Not even visible, really.
                case TiletypeShape.SAPLING:
                case TiletypeShape.SHRUB:
                case TiletypeShape.BRANCH:
                case TiletypeShape.TRUNK_BRANCH:
                    return true;
                default:
                    return false;
                }
            }
        }
        public bool isSolidBase
        {
            get
            {
                switch (shape)
                {
                    case TiletypeShape.FLOOR:
                    case TiletypeShape.BOULDER:
                    case TiletypeShape.PEBBLES:
                    case TiletypeShape.WALL:
                    case TiletypeShape.FORTIFICATION:
                    case TiletypeShape.STAIR_UP:
                    case TiletypeShape.RAMP:
                    case TiletypeShape.BROOK_BED:
                    case TiletypeShape.TREE_SHAPE:
                    case TiletypeShape.SAPLING:
                    case TiletypeShape.SHRUB:
                        return true;
                    default:
                        return false;
                }
            }
        }
        public MapDataStore.Tile? North {
            get {
                return container[position.x, position.y - 1, position.z];
            }
        }
        public MapDataStore.Tile? South {
            get {
                return container[position.x, position.y + 1, position.z];
            }
        }
        public MapDataStore.Tile? East {
            get {
                return container[position.x + 1, position.y, position.z];
            }
        }
        public MapDataStore.Tile? West {
            get {
                return container[position.x - 1, position.y, position.z];
            }
        }
        public MapDataStore.Tile? Up {
            get {
                return container[position.x, position.y, position.z + 1];
            }
        }
        public MapDataStore.Tile? Down {
            get {
                return container[position.x, position.y, position.z - 1];
            }
        }

        //moved from RampConfiguration
        //big look up table
        static readonly byte[] rampblut =
        // generated by blutmaker.py
        {
            1 ,  2 ,  8 ,  2 ,  4 , 12 ,  4 , 12 ,  9 ,  2 , 21 ,  2 ,  4 , 12 ,  4 , 12 ,
            5 , 16 ,  5 , 16 , 13 , 13 , 13 , 12 ,  5 , 16 ,  5 , 16 , 13 , 13 , 13 , 16 ,
            7 ,  2 , 14 ,  2 ,  4 , 12 ,  4 , 12 , 20 , 26 , 25 , 26 ,  4 , 12 ,  4 , 12 ,
            5 , 16 ,  5 , 16 , 13 , 16 , 13 , 16 ,  5 , 16 ,  5 , 16 , 13 , 16 , 13 , 16 ,
            3 , 10 ,  3 , 10 , 17 , 12 , 17 , 12 ,  3 , 10 , 26 , 10 , 17 , 17 , 17 , 17 ,
            11 , 10 , 11 , 16 , 11 , 26 , 17 , 12 , 11 , 16 , 11 , 16 , 13 , 13 , 17 , 16 ,
            3 , 10 ,  3 , 10 , 17 , 17 , 17 , 17 ,  3 , 10 , 26 , 10 , 17 , 17 , 17 , 17 ,
            11 , 11 , 11 , 16 , 11 , 11 , 17 , 14 , 11 , 16 , 11 , 16 , 17 , 17 , 17 , 13 ,
            6 ,  2 , 19 ,  2 ,  4 , 12 ,  4 , 12 , 15 ,  2 , 24 ,  2 ,  4 , 12 ,  4 , 12 ,
            5 , 16 , 26 , 16 , 13 , 16 , 13 , 16 ,  5 , 16 , 26 , 16 , 13 , 16 , 13 , 16 ,
            18 ,  2 , 22 ,  2 , 26 , 12 , 26 , 12 , 23 , 26 , 26 , 26 , 26 , 12 , 26 , 12 ,
            5 , 16 , 26 , 16 , 13 , 16 , 13 , 16 ,  5 , 16 , 26 , 16 , 13 , 16 , 13 , 16 ,
            3 , 10 ,  3 , 10 , 17 , 10 , 17 , 17 ,  3 , 10 , 26 , 10 , 17 , 17 , 17 , 17 ,
            11 , 10 , 11 , 16 , 17 , 10 , 17 , 17 , 11 , 16 , 11 , 16 , 17 , 15 , 17 , 12 ,
            3 , 10 ,  3 , 10 , 17 , 17 , 17 , 17 ,  3 , 10 , 26 , 10 , 17 , 17 , 17 , 17 ,
            11 , 16 , 11 , 16 , 17 , 16 , 17 , 10 , 11 , 16 , 11 , 16 , 17 , 11 , 17 , 26
        };
        public void CalculateRampType()
        {
            int ramplookup = 0;
            if (North != null &&
                North.Value.Up != null &&
                North.Value.isWall &&
                North.Value.Up.Value.isFloor)
            {
                ramplookup ^= 1;
            }
            if (North != null &&
                North.Value.East != null &&
                North.Value.East.Value.Up != null &&
                North.Value.East.Value.isWall &&
                North.Value.East.Value.Up.Value.isFloor)
            {
                ramplookup ^= 2;
            }
            if (East != null &&
                East.Value.Up != null &&
                East.Value.isWall &&
                East.Value.Up.Value.isFloor)
            {
                ramplookup ^= 4;
            }
            if (South != null &&
                South.Value.East != null &&
                South.Value.East.Value.Up != null &&
                South.Value.East.Value.isWall &&
                South.Value.East.Value.Up.Value.isFloor)
            {
                ramplookup ^= 8;
            }
            if (South != null &&
                South.Value.Up != null &&
                South.Value.isWall &&
                South.Value.Up.Value.isFloor)
            {
                ramplookup ^= 16;
            }
            if (South != null &&
                South.Value.West != null &&
                South.Value.West.Value.Up != null &&
                South.Value.West.Value.isWall &&
                South.Value.West.Value.Up.Value.isFloor)
            {
                ramplookup ^= 32;
            }
            if (West != null &&
                West.Value.Up != null &&
                West.Value.isWall &&
                West.Value.Up.Value.isFloor)
            {
                ramplookup ^= 64;
            }
            if (North != null &&
                North.Value.West != null &&
                North.Value.West.Value.Up != null &&
                North.Value.West.Value.isWall &&
                North.Value.West.Value.Up.Value.isFloor)
            {
                ramplookup ^= 128;
            }
            // creation should ensure in range
            if (ramplookup > 0)
            {
                rampType = rampblut[ramplookup];
                return;
            }

            if (North != null &&
                North.Value.isWall)
            {
                ramplookup ^= 1;
            }
            if (North != null &&
                North.Value.East != null &&
                North.Value.East.Value.isWall)
            {
                ramplookup ^= 2;
            }
            if (East != null &&
                East.Value.isWall)
            {
                ramplookup ^= 4;
            }
            if (South != null &&
                South.Value.East != null &&
                South.Value.East.Value.isWall)
            {
                ramplookup ^= 8;
            }
            if (South != null &&
                South.Value.isWall)
            {
                ramplookup ^= 16;
            }
            if (South != null &&
                South.Value.West != null &&
                South.Value.West.Value.isWall)
            {
                ramplookup ^= 32;
            }
            if (West != null &&
                West.Value.isWall)
            {
                ramplookup ^= 64;
            }
            if (North != null &&
                North.Value.West != null &&
                North.Value.West.Value.isWall)
            {
                ramplookup ^= 128;
            }
            rampType = rampblut[ramplookup];
            return;
        }

        public Directions WallSides
        {
            get
            {
                Directions wallSide = Directions.None;
                if (North != null &&
                    North.Value.isWall)
                {
                    wallSide |= Directions.North;
                }
                if (North != null &&
                    North.Value.East != null &&
                    North.Value.East.Value.isWall)
                {
                    wallSide |= Directions.NorthEast;
                }
                if (East != null &&
                    East.Value.isWall)
                {
                    wallSide |= Directions.East;
                }
                if (South != null &&
                    South.Value.East != null &&
                    South.Value.East.Value.isWall)
                {
                    wallSide |= Directions.SouthEast;
                }
                if (South != null &&
                    South.Value.isWall)
                {
                    wallSide |= Directions.South;
                }
                if (South != null &&
                    South.Value.West != null &&
                    South.Value.West.Value.isWall)
                {
                    wallSide |= Directions.SouthWest;
                }
                if (West != null &&
                    West.Value.isWall)
                {
                    wallSide |= Directions.West;
                }
                if (North != null &&
                    North.Value.West != null &&
                    North.Value.West.Value.isWall)
                {
                    wallSide |= Directions.NorthWest;
                }
                return wallSide;
            }
        }
        public Directions WallBuildingSides
        {
            get
            {
                Directions wallSide = Directions.None;
                if (North != null &&
                    North.Value.isWallBuilding)
                {
                    wallSide |= Directions.North;
                }
                if (North != null &&
                    North.Value.East != null &&
                    North.Value.East.Value.isWallBuilding)
                {
                    wallSide |= Directions.NorthEast;
                }
                if (East != null &&
                    East.Value.isWallBuilding)
                {
                    wallSide |= Directions.East;
                }
                if (South != null &&
                    South.Value.East != null &&
                    South.Value.East.Value.isWallBuilding)
                {
                    wallSide |= Directions.SouthEast;
                }
                if (South != null &&
                    South.Value.isWallBuilding)
                {
                    wallSide |= Directions.South;
                }
                if (South != null &&
                    South.Value.West != null &&
                    South.Value.West.Value.isWallBuilding)
                {
                    wallSide |= Directions.SouthWest;
                }
                if (West != null &&
                    West.Value.isWallBuilding)
                {
                    wallSide |= Directions.West;
                }
                if (North != null &&
                    North.Value.West != null &&
                    North.Value.West.Value.isWallBuilding)
                {
                    wallSide |= Directions.NorthWest;
                }
                return wallSide;
            }
        }
    }
}
