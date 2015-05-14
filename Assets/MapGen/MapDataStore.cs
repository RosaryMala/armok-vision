using UnityEngine;
using System.Collections;
using DFHack;
using System.Collections.Generic;
using RemoteFortressReader;

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
            throw new UnityException("Can't make map slice without any tiles");
        }
        SliceSize = sliceSize;
        tiles = new Tile[SliceSize.x, SliceSize.y, SliceSize.z];
        tilesPresent = new BitArray(PresentIndex(SliceSize.x-1, SliceSize.y-1, SliceSize.z-1)+1);
        Reset();
    }

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
                    target.tiles[x, y, z] = tiles[localNewSliceOrigin.x+x, localNewSliceOrigin.y+y, localNewSliceOrigin.z+z];
                    target.tiles[x, y, z].container = target;
                }
            }
        }
        target.SliceOrigin = newSliceOrigin;
    }

    public MapDataStore CopySlice(DFCoord newSliceOrigin, DFCoord newSliceSize) {
        MapDataStore target = new MapDataStore(newSliceOrigin, newSliceSize);
        CopySliceTo(newSliceOrigin, newSliceSize, target);
        return target;
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
                int netIndex = xx + (yy * 16);
                tilesPresent[PresentIndex(localCoord.x, localCoord.y, localCoord.z)] = true;
                if (setTiles) {
                    tiles[localCoord.x, localCoord.y, localCoord.z].tileType = block.tiles[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].material = block.materials[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].base_material = block.base_materials[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].layer_material = block.layer_materials[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].vein_material = block.vein_materials[netIndex];
                }
                if (setLiquids) {
                    tiles[localCoord.x, localCoord.y, localCoord.z].waterLevel = block.water[netIndex];
                    tiles[localCoord.x, localCoord.y, localCoord.z].magmaLevel = block.magma[netIndex];
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
                           int? magmaLevel = null)
    {
        DFCoord local = WorldToLocalSpace(coord);
        if (!InSliceBoundsLocal(local.x, local.y, local.z)) {
            throw new UnityException("Can't modify tile outside of slice");
        }
        tilesPresent[PresentIndex(local.x, local.y, local.z)] = true;
        tiles[local.x, local.y, local.z].Modify(tileType, material, base_material, layer_material, vein_material, waterLevel, magmaLevel);
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
            waterLevel = default(int);
            magmaLevel = default(int);
        }

        public MapDataStore container;
        public DFCoord position;
        public int tileType;
        public MatPairStruct material;
        public MatPairStruct base_material;
        public MatPairStruct layer_material;
        public MatPairStruct vein_material;
        public int waterLevel;
        public int magmaLevel;

        public TiletypeShape shape { get { return tiletypeTokenList [tileType].shape; } }
        public TiletypeMaterial tiletypeMaterial { get { return tiletypeTokenList [tileType].material; } }
        public TiletypeSpecial special { get { return tiletypeTokenList [tileType].special; } }
        public TiletypeVariant variant { get { return tiletypeTokenList [tileType].variant; } }
        public string direction { get { return tiletypeTokenList [tileType].direction; } }

        public void Modify (int? tileType = null,
                           MatPairStruct? material = null,
                           MatPairStruct? base_material = null,
                           MatPairStruct? layer_material = null,
                           MatPairStruct? vein_material = null,
                           int? waterLevel = null,
                           int? magmaLevel = null)
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
        }
        public bool isWall {
            get {
                switch (shape) {
                case TiletypeShape.WALL:
                case TiletypeShape.FORTIFICATION:
                case TiletypeShape.BROOK_BED:
                case TiletypeShape.TREE_SHAPE:
                    return true;
                default:
                    return false;
                }
            }
        }
        public bool isFloor {
            get {
                switch (shape) {
                case TiletypeShape.RAMP:
                case TiletypeShape.FLOOR:
                case TiletypeShape.BOULDER:
                case TiletypeShape.PEBBLES:
                case TiletypeShape.BROOK_TOP:
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
        public MapDataStore.Tile? north {
            get {
                return container[position.x, position.y - 1, position.z];
            }
        }
        public MapDataStore.Tile? south {
            get {
                return container[position.x, position.y + 1, position.z];
            }
        }
        public MapDataStore.Tile? east {
            get {
                return container[position.x + 1, position.y, position.z];
            }
        }
        public MapDataStore.Tile? west {
            get {
                return container[position.x - 1, position.y, position.z];
            }
        }
        public MapDataStore.Tile? up {
            get {
                return container[position.x, position.y, position.z + 1];
            }
        }
        public MapDataStore.Tile? down {
            get {
                return container[position.x, position.y, position.z - 1];
            }
        }
    }
}
