using DFHack;
using RemoteFortressReader;
using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
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
    DFCoord _sliceOrigin;
    public DFCoord SliceOrigin
    {
        get
        {
            return _sliceOrigin;
        }
        private set
        {
            _sliceOrigin = value;
            MinCoord = _sliceOrigin - new DFCoord(1,1,1);
            MaxCoord = _sliceOrigin + SliceSize + new DFCoord(1, 1, 1);
        }
    }
    public DFCoord MinCoord { get; private set; }
    public DFCoord MaxCoord { get; private set; }
    // The data
    Tile[,,] _tiles;

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
        _tiles = new Tile[SliceSize.x + 2, SliceSize.y + 2, SliceSize.z + 2]; //add two so we get edges.
        Reset();
    }

    public MapDataStore(BlockCoord block) : this(block.ToDFCoord(), BLOCK_SIZE){}

    public bool CopySliceTo(DFCoord newSliceOrigin, DFCoord newSliceSize, MapDataStore target) {
        bool success = false;
        if (newSliceSize != target.SliceSize) {
            throw new UnityException("Mismatched slice sizes");
        }
        var localNewSliceOrigin = WorldToLocalSpace(newSliceOrigin);
        if (!InSliceBoundsLocal(localNewSliceOrigin) || !InSliceBoundsLocal(localNewSliceOrigin + newSliceSize - new DFCoord(1,1,1))) {
            throw new UnityException("Can't slice outside of our slice bounds");
        }
        target.SliceOrigin = newSliceOrigin;
        for (int x = target.MinCoord.x; x < target.MaxCoord.x; x++) {
            for (int y = target.MinCoord.y; y < target.MaxCoord.y; y++) {
                for (int z = target.MinCoord.z; z < target.MaxCoord.z; z++) {
                    if (this[x, y, z] == null)
                    {
                        target[x, y, z] = null;
                        continue;
                    }
                    //pre-calculate it before we copy, because afterwards we won't have contextual data.
                    if (this[x,y,z].shape == TiletypeShape.RAMP)
                        this[x, y, z].CalculateRampType();
                    target[x, y, z] = this[x,y,z];
                    success = true;
                }
            }
        }
        return success;
    }

    const int MAXIMUM_CHECKS = 5000;

    /// <summary>
    /// A big method, but pretty simple.
    /// Walk through tiles (starting in current one);
    /// in each tile, check if the ray is actively hitting something.
    /// If it's not, find the wall of the tile the ray exits through,
    /// go to that tile, and repeat.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="tileCoord"></param>
    /// <param name="unityCoord"></param>
    /// <returns>True, if hit.</returns>
    public static bool FindCurrentTarget(Ray ray, out DFCoord tileCoord, out Vector3 unityCoord)
    {
        if (!HitsMapCube(ray))
        {
            tileCoord = default(DFCoord);
            unityCoord = default(Vector3);
            return false;
        }

        // In each tile, we find its bottom corner, and then add these
        // values to find the coordinates of the walls.
        // If the ray increases on this axis, the offset will be the
        // width of the tile along that axis; if the ray decreases,
        // the offset will be 0 (since we're already on that wall.)
        float xWallOffset, yWallOffset, zWallOffset;
        // When we pass through a tile and hit this wall, how do we increment
        // our DFCoord?
        DFCoord xHitIncrement, yHitIncrement, zHitIncrement;
        if (ray.direction.x > 0)
        {
            xWallOffset = GameMap.tileWidth;
            xHitIncrement = new DFCoord(1, 0, 0);
        }
        else
        {
            xWallOffset = 0;
            xHitIncrement = new DFCoord(-1, 0, 0);
        }
        if (ray.direction.z > 0)
        {
            zWallOffset = GameMap.tileWidth;
            zHitIncrement = new DFCoord(0, -1, 0);
        }
        else
        {
            zWallOffset = 0;
            zHitIncrement = new DFCoord(0, 1, 0);
        }
        if (ray.direction.y > 0)
        {
            yWallOffset = GameMap.tileHeight;
            yHitIncrement = new DFCoord(0, 0, 1);
        }
        else
        {
            yWallOffset = 0;
            yHitIncrement = new DFCoord(0, 0, -1);
        }

        // If this is true and we go onto a tile outside the map,
        // we stop iterating (since we can't hit anything.)
        bool haveHitMap = false;

        // The coordinate we start at.
        DFCoord currentCoord = GameMap.UnityToDFCoord(ray.origin);

        // The coordinate of the last tile wall intersection.
        Vector3 lastHit = ray.origin;

        // Cheap hack to keep from looping forever if we screw up somehow.
        for (int _insurance = 0; _insurance < MAXIMUM_CHECKS; _insurance++)
        {
            // Make sure we don't move backwards somehow.
            if ((lastHit.x - ray.origin.x) / ray.direction.x < 0)
            {
                throw new UnityException("Negative distance multiplier?");
            }

            // Get the corner of the current tile.
            Vector3 cornerCoord = GameMap.DFtoUnityBottomCorner(currentCoord);

            // Are we in the selectable area of the map?
            if (!InMapBounds(currentCoord) || GameMap.Instance.PosZ <= currentCoord.z)
            {
                // No.
                if (haveHitMap)
                {
                    // But we have been before;
                    // we've entered and exited the map without hitting anything.
                    tileCoord = default(DFCoord);
                    unityCoord = default(Vector3);
                    return false;
                }
            }
            else
            {
                // We are in the map.
                haveHitMap = true;

                Tile currentTile = Main[currentCoord.x, currentCoord.y, currentCoord.z];
                // Are we in a real tile?
                if (currentTile != null)
                {
                    // Yes.
                    switch (currentTile.shape)
                    {
                        case TiletypeShape.EMPTY:
                        case TiletypeShape.NO_SHAPE:
                            // We're not hitting anything, though.
                            break;
                        //case RemoteFortressReader.TiletypeShape.SHRUB:
                        //case RemoteFortressReader.TiletypeShape.SAPLING:
                        case TiletypeShape.WALL:
                        case TiletypeShape.FORTIFICATION:
                        //case RemoteFortressReader.TiletypeShape.TRUNK_BRANCH:
                        case TiletypeShape.TWIG:
                            // We must be hitting things.
                            // (maybe adjust shrub, saplings out of this group?)
                            tileCoord = currentCoord;
                            unityCoord = lastHit;
                            return true;
                        case TiletypeShape.RAMP:
                            {
                                // Check if we're in the ramp.
                                // (that we're in the tile is implied.)
                                if (Between(cornerCoord.y, lastHit.y, cornerCoord.y + GameMap.floorHeight + (GameMap.tileHeight / 2)))
                                {
                                    tileCoord = currentCoord;
                                    unityCoord = lastHit;
                                    return true;
                                }
                                // Check if we enter the ramp; same way we check wall intersections.
                                float floorY = cornerCoord.y + GameMap.floorHeight + (GameMap.tileHeight / 2);
                                float toFloorMult = (floorY - ray.origin.y) / ray.direction.y;
                                Vector3 floorIntercept = ray.origin + ray.direction * toFloorMult;
                                if (Between(cornerCoord.x, floorIntercept.x, cornerCoord.x + GameMap.tileWidth) &&
                                    Between(cornerCoord.z, floorIntercept.z, cornerCoord.z + GameMap.tileWidth))
                                {
                                    tileCoord = currentCoord;
                                    unityCoord = lastHit;
                                    return true;
                                }
                            }
                            break;
                        case TiletypeShape.FLOOR:
                        case TiletypeShape.BOULDER:
                        case TiletypeShape.PEBBLES:
                        case TiletypeShape.BROOK_TOP:
                        case TiletypeShape.SAPLING:
                        case TiletypeShape.SHRUB:
                        case TiletypeShape.BRANCH:
                        case TiletypeShape.TRUNK_BRANCH:
                            {
                                // Check if we're in the floor.
                                // (that we're in the tile is implied.)
                                if (Between(cornerCoord.y, lastHit.y, cornerCoord.y + GameMap.floorHeight))
                                {
                                    tileCoord = currentCoord;
                                    unityCoord = lastHit;
                                    return true;
                                }
                                // Check if we enter the floor; same way we check wall intersections.
                                float floorY = cornerCoord.y + GameMap.floorHeight;
                                float toFloorMult = (floorY - ray.origin.y) / ray.direction.y;
                                Vector3 floorIntercept = ray.origin + ray.direction * toFloorMult;
                                if (Between(cornerCoord.x, floorIntercept.x, cornerCoord.x + GameMap.tileWidth) &&
                                    Between(cornerCoord.z, floorIntercept.z, cornerCoord.z + GameMap.tileWidth))
                                {
                                    tileCoord = currentCoord;
                                    unityCoord = lastHit;
                                    return true;
                                }
                            }
                            break;
                    }
                }
            }
            // Didn't hit anything in the tile; figure out which wall we're hitting & walk to that tile.
            {
                float xMult = (cornerCoord.x + xWallOffset - ray.origin.x) / ray.direction.x;
                Vector3 xIntercept = ray.origin + ray.direction * xMult;
                if (Between(cornerCoord.z, xIntercept.z, cornerCoord.z + GameMap.tileWidth) &&
                    Between(cornerCoord.y, xIntercept.y, cornerCoord.y + GameMap.tileHeight))
                {
                    lastHit = xIntercept;
                    currentCoord += xHitIncrement;
                    continue;
                }
            }
            {
                float zMult = (cornerCoord.z + zWallOffset - ray.origin.z) / ray.direction.z;
                Vector3 zIntercept = ray.origin + ray.direction * zMult;
                if (Between(cornerCoord.x, zIntercept.x, cornerCoord.x + GameMap.tileWidth) &&
                    Between(cornerCoord.y, zIntercept.y, cornerCoord.y + GameMap.tileHeight))
                {
                    lastHit = zIntercept;
                    currentCoord += zHitIncrement;
                    continue;
                }
            }
            {
                float yMult = (cornerCoord.y + yWallOffset - ray.origin.y) / ray.direction.y;
                Vector3 yIntercept = ray.origin + ray.direction * yMult;
                if (cornerCoord.x <= yIntercept.x && yIntercept.x <= cornerCoord.x + GameMap.tileWidth &&
                    cornerCoord.z <= yIntercept.z && yIntercept.z <= cornerCoord.z + GameMap.tileWidth)
                {
                    lastHit = yIntercept;
                    currentCoord += yHitIncrement;
                    continue;
                }
            }
            // We haven't found a wall to hit.
            // This shouldn't happen, but occasionally does.
            //throw new UnityException("Didn't hit any tile walls?");
        }

        // We went the maximum amount of time without hitting anything
        tileCoord = default(DFCoord);
        unityCoord = default(Vector3);
        return false;
    }

    static bool Between(float lower, float t, float upper)
    {
        return lower <= t && t <= upper;
    }

    // Check if a ray could possibly hit the game map at all
    static bool HitsMapCube(Ray ray)
    {
        if (Main == null)
            return false; //there's no cube to hit.
        Vector3 lowerLimits = GameMap.DFtoUnityBottomCorner(new DFCoord(0, 0, 0));
        Vector3 upperLimits = GameMap.DFtoUnityBottomCorner(new DFCoord(
            MapSize.x - 1,
            MapSize.y - 1,
            MapSize.z - 1
            )) + new Vector3(GameMap.tileWidth, GameMap.tileHeight, GameMap.tileWidth);

        // Multipliers to scale the ray to hit the different walls of the cube
        float tx1 = (lowerLimits.x - ray.origin.x) / ray.direction.x;
        float tx2 = (upperLimits.x - ray.origin.x) / ray.direction.x;
        float ty1 = (lowerLimits.y - ray.origin.y) / ray.direction.y;
        float ty2 = (upperLimits.y - ray.origin.y) / ray.direction.y;
        float tz1 = (lowerLimits.z - ray.origin.z) / ray.direction.z;
        float tz2 = (upperLimits.z - ray.origin.z) / ray.direction.z;

        float tMin = Mathf.Min(tx1, tx2, ty1, ty2, tz1, tz2);
        float tMax = Mathf.Max(tx1, tx2, ty1, ty2, tz1, tz2);

        // If tMax < 0, cube is entirely behind us; 
        // if tMin > tMax, we don't intersect the cube at all
        return tMin < tMax && 0 < tMax;
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

    private static Tile GetTileForWriting(DFCoord pos)
    {
        if (!Main.InSliceBounds(pos))
            return null;
        if(Main[pos] == null)
        {
            Main[pos] = new Tile(Main, pos);
        }
        return Main[pos];
    }

    public static void StoreTiles(MapBlock block, out bool setTiles, out bool setLiquids, out bool setSpatters) {
        setTiles = block.tiles.Count > 0;
        setLiquids = block.water.Count > 0 || block.magma.Count > 0;
        setSpatters = block.spatterPile.Count > 0;

        if (!(setTiles || setLiquids || setSpatters))
            return;

        for (int yy = 0; yy < 16; yy++)
            for (int xx = 0; xx < 16; xx++)
            {
                DFCoord worldCoord = new DFCoord(block.map_x + xx, block.map_y + yy, block.map_z);
                var tile = GetTileForWriting(worldCoord);
                if (tile == null)
                    return;
                int netIndex = xx + (yy * 16);
                if (block.tiles.Count > 0)
                {
                    tile.tileType = block.tiles[netIndex];
                    tile.material = block.materials[netIndex];
                    tile.base_material = block.base_materials[netIndex];
                    tile.layer_material = block.layer_materials[netIndex];
                    tile.vein_material = block.vein_materials[netIndex];
                    if (block.construction_items != null && block.construction_items.Count > netIndex)
                        tile.construction_item = block.construction_items[netIndex];
                    else
                        tile.construction_item = new MatPairStruct(-1, -1);
                    if (block.tree_percent != null && block.tree_percent.Count > netIndex)
                    {
                        tile.trunkPercent = (byte)block.tree_percent[netIndex];
                        tile.positionOnTree = new DFCoord(block.tree_x[netIndex], block.tree_y[netIndex], block.tree_z[netIndex]);
                    }
                    else
                    {
                        tile.trunkPercent = 0;
                        tile.positionOnTree = new DFCoord(0,0,0);
                    }
                }
                if (setLiquids)
                {
                    tile.waterLevel = block.water[netIndex];
                    tile.magmaLevel = block.magma[netIndex];
                    if (tile.Hidden != block.hidden[netIndex])
                    {
                        tile.Hidden  = block.hidden[netIndex];
                        setTiles = true;
                    }
                    if (block.tile_dig_designation != null && block.tile_dig_designation.Count > netIndex)
                    {
                        if (tile.digDesignation != block.tile_dig_designation[netIndex])
                        {
                            tile.digDesignation = block.tile_dig_designation[netIndex];
                            setTiles = true;
                        }
                    }
                }
                if(setSpatters)
                {
                    tile.spatters = block.spatterPile[netIndex].spatters;
                }
            }
    }

    private static DFCoord2d GetRotatedLocalCoord(DFCoord worldCoord, BuildingInstance building)
    {
        switch (building.direction)
        {
            case BuildingDirection.NORTH:
                return new DFCoord2d(worldCoord.x - building.pos_x_min, worldCoord.y - building.pos_y_min);
            case BuildingDirection.EAST:
                return new DFCoord2d(worldCoord.y - building.pos_y_min, building.pos_x_max - worldCoord.x);
            case BuildingDirection.SOUTH:
                return new DFCoord2d(building.pos_x_max - worldCoord.x, building.pos_y_max - worldCoord.y);
            case BuildingDirection.WEST:
                return new DFCoord2d(building.pos_y_max - worldCoord.y, worldCoord.x - building.pos_x_min);
            default:
                return new DFCoord2d(worldCoord.x - building.pos_x_min, worldCoord.y - building.pos_y_min);
        }
    }

    public bool InSliceBounds(DFCoord loc) {
        return InSliceBoundsLocal(WorldToLocalSpace(loc));
    }

    // Things to read and modify the map
    // Note: everything takes coordinates in world / DF space
    public Tile this[int x, int y, int z]
    {
        get
        {
            return this[new DFCoord(x, y, z)];
        }
        private set
        {
            this[new DFCoord(x, y, z)] = value;
        }
    }

    public Tile this[DFCoord coord]
    {
        get
        {
            DFCoord local = WorldToLocalSpace(coord);
            if (InSliceBoundsLocal(local.x, local.y, local.z))
            {
                return _tiles[local.x, local.y, local.z];
            }
            else
            {
                return null;
            }
        }
        private set
        {
            DFCoord local = WorldToLocalSpace(coord);
            if (InSliceBoundsLocal(local.x, local.y, local.z))
            {
                if (value == null)
                    _tiles[local.x, local.y, local.z] = value;
                else if (value.container == null)
                {
                    value.container = this;
                    _tiles[local.x, local.y, local.z] = value;
                }
                else if (value.container != this)
                {
                    if (_tiles[local.x, local.y, local.z] == null)
                        _tiles[local.x, local.y, local.z] = new Tile(value);
                    else
                        _tiles[local.x, local.y, local.z].CopyFrom(value);
                    Tile tile = _tiles[local.x, local.y, local.z];
                    tile.container = this;
                    tile.position = coord;
                }
                else
                    _tiles[local.x, local.y, local.z] = value;
            }
        }
    }

    public int GetLiquidLevel(DFCoord coord, int liquidIndex) {
        var tile = this[coord];
        if (tile == null) return 0;
        if (tile.Hidden) return 0;
        switch (liquidIndex) {
        case WATER_INDEX:
            return tile.waterLevel;
        case MAGMA_INDEX:
            return tile.magmaLevel;
        default:
            throw new UnityException("No liquid with index "+liquidIndex);
        }
    }

    public void Reset() {
        for (int x = 0; x < _tiles.GetLength(0); x++) {
            for (int y = 0; y < _tiles.GetLength(1); y++) {
                for (int z = 0; z < _tiles.GetLength(2); z++) {
                    _tiles[x,y,z] = null;
                }
            }
        }
    }

    // Helpers
    public DFCoord WorldToLocalSpace(DFCoord coord)
    {
        return coord - SliceOrigin + new DFCoord(1, 1, 1);
    }
    public DFCoord LocalToWorldSpace(DFCoord coord)
    {
        return coord + SliceOrigin - new DFCoord(1, 1, 1);
    }
    // These take local space coordinates:
    bool InSliceBoundsLocal(int x, int y, int z)
    {
        return
            0 <= x && x < SliceSize.x + 2 &&
            0 <= y && y < SliceSize.y + 2 &&
            0 <= z && z < SliceSize.z + 2;
    }
    bool InSliceBoundsLocal(DFCoord coord) {
        return InSliceBoundsLocal(coord.x, coord.y, coord.z);
    }
    int PresentIndex(int x, int y, int z) {
        return x * SliceSize.y * SliceSize.z + y * SliceSize.z + z;
    }

    public enum CollisionState
    {
        None,
        Stairs,
        Water,
        Solid
    }

    public CollisionState CheckCollision(Vector3 pos)
    {
        var dfPos = GameMap.UnityToDFCoord(pos);
        var localPos = pos - GameMap.DFtoUnityCoord(dfPos);
        var tile = this[dfPos];
        if (tile == null)
            return CollisionState.None;
        var shape = tile.shape;

        var state = CollisionState.None;

        switch (shape)
        {
            case TiletypeShape.NO_SHAPE:
            case TiletypeShape.EMPTY:
            case TiletypeShape.ENDLESS_PIT:
            case TiletypeShape.TWIG:
                state = CollisionState.None;
                break;
            case TiletypeShape.FLOOR:
            case TiletypeShape.BOULDER:
            case TiletypeShape.PEBBLES:
            case TiletypeShape.BROOK_TOP:
            case TiletypeShape.SAPLING:
            case TiletypeShape.SHRUB:
            case TiletypeShape.BRANCH:
            case TiletypeShape.TRUNK_BRANCH:
                if (localPos.y < 0.5f)
                    state = CollisionState.Solid;
                else
                    state = CollisionState.None;
                break;
            case TiletypeShape.WALL:
            case TiletypeShape.FORTIFICATION:
            case TiletypeShape.BROOK_BED:
            case TiletypeShape.TREE_SHAPE:
                state = CollisionState.Solid;
                break;
            case TiletypeShape.STAIR_UP:
                if (localPos.y < 0.5f)
                    state = CollisionState.Solid;
                else
                    state = CollisionState.Stairs;
                break;
            case TiletypeShape.STAIR_DOWN:
                if (localPos.y < 0.5f)
                    state = CollisionState.Stairs;
                else
                    state = CollisionState.None;
                break;
            case TiletypeShape.STAIR_UPDOWN:
                state = CollisionState.Stairs;
                break;
            case TiletypeShape.RAMP:
                if (localPos.y < 0.5f)
                    state = CollisionState.Solid;
                else
                    state = CollisionState.None;

                break;
            case TiletypeShape.RAMP_TOP:
                break;
            default:
                break;
        }
        return state;
    }

    DFCoord2d SnapDirection(Vector2 dir)
    {
        if(Mathf.Abs(dir.x) > Math.Abs(dir.y))
        {
            //x is the dominant direction
            if (dir.x > 0)
                return new DFCoord2d(1, 0);
            else
                return new DFCoord2d(-1, 0);
        }
        else
        {
            //y is the dominant dir.
            if (dir.y <= 0)
                return new DFCoord2d(0, 1);
            else
                return new DFCoord2d(0, -1);
        }
    }

    // The data for a single tile of the map.
    // Nested struct because it depends heavily on its container.
    public class Tile
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
            Hidden = false;
            trunkPercent = 0;
            positionOnTree = default(DFCoord);
            digDesignation = TileDigDesignation.NO_DIG;
            spatters = null;
        }

        public Tile(Tile orig)
        {
            CopyFrom(orig);
        }

        public void CopyFrom(Tile orig)
        {
            container = orig.container;
            position = orig.position;
            tileType = orig.tileType;
            material = orig.material;
            base_material = orig.base_material;
            layer_material = orig.layer_material;
            vein_material = orig.vein_material;
            construction_item = orig.construction_item;
            waterLevel = orig.waterLevel;
            magmaLevel = orig.magmaLevel;
            RampType = orig.RampType;
            Hidden = orig.Hidden;
            trunkPercent = orig.trunkPercent;
            positionOnTree = orig.positionOnTree;
            digDesignation = orig.digDesignation;
            spatters = orig.spatters;
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
        private bool _hidden;
        public bool Hidden
        {
            get
            {
                if (GameSettings.Instance.rendering.showHiddenTiles)
                    return false;
                else
                    return _hidden;
            }
            set
            {
                _hidden = value;
            }
        }
        public byte trunkPercent;
        public DFCoord positionOnTree;
        public TileDigDesignation digDesignation;
        public List<Spatter> spatters;

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

        public bool isWall {
            get {
                switch (shape) {
                case TiletypeShape.WALL:
                //case TiletypeShape.FORTIFICATION: //dwarfs can't go through, but visibly, they're not solid.
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
                    //|| buildingType.building_type == 8 //Door
                    //|| buildingType.building_type == 9 //Floodgate
                    //|| buildingType.building_type == 16 //WindowGlass
                    //|| buildingType.building_type == 17 //WindowGem
                    || shape == TiletypeShape.FORTIFICATION //since isWall doesn't handle this.
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
        public MapDataStore.Tile North {
            get {
                return container[position.x, position.y - 1, position.z];
            }
        }
        public MapDataStore.Tile South {
            get {
                return container[position.x, position.y + 1, position.z];
            }
        }
        public MapDataStore.Tile East {
            get {
                return container[position.x + 1, position.y, position.z];
            }
        }
        public MapDataStore.Tile West {
            get {
                return container[position.x - 1, position.y, position.z];
            }
        }
        public MapDataStore.Tile Up {
            get {
                return container[position.x, position.y, position.z + 1];
            }
        }
        public MapDataStore.Tile Down {
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
                North.Up != null &&
                North.isWall &&
                North.Up.isFloor)
            {
                ramplookup ^= 1;
            }
            if (North != null &&
                North.East != null &&
                North.East.Up != null &&
                North.East.isWall &&
                North.East.Up.isFloor)
            {
                ramplookup ^= 2;
            }
            if (East != null &&
                East.Up != null &&
                East.isWall &&
                East.Up.isFloor)
            {
                ramplookup ^= 4;
            }
            if (South != null &&
                South.East != null &&
                South.East.Up != null &&
                South.East.isWall &&
                South.East.Up.isFloor)
            {
                ramplookup ^= 8;
            }
            if (South != null &&
                South.Up != null &&
                South.isWall &&
                South.Up.isFloor)
            {
                ramplookup ^= 16;
            }
            if (South != null &&
                South.West != null &&
                South.West.Up != null &&
                South.West.isWall &&
                South.West.Up.isFloor)
            {
                ramplookup ^= 32;
            }
            if (West != null &&
                West.Up != null &&
                West.isWall &&
                West.Up.isFloor)
            {
                ramplookup ^= 64;
            }
            if (North != null &&
                North.West != null &&
                North.West.Up != null &&
                North.West.isWall &&
                North.West.Up.isFloor)
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
                North.isWall)
            {
                ramplookup ^= 1;
            }
            if (North != null &&
                North.East != null &&
                North.East.isWall)
            {
                ramplookup ^= 2;
            }
            if (East != null &&
                East.isWall)
            {
                ramplookup ^= 4;
            }
            if (South != null &&
                South.East != null &&
                South.East.isWall)
            {
                ramplookup ^= 8;
            }
            if (South != null &&
                South.isWall)
            {
                ramplookup ^= 16;
            }
            if (South != null &&
                South.West != null &&
                South.West.isWall)
            {
                ramplookup ^= 32;
            }
            if (West != null &&
                West.isWall)
            {
                ramplookup ^= 64;
            }
            if (North != null &&
                North.West != null &&
                North.West.isWall)
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
                    North.isWall)
                {
                    wallSide |= Directions.North;
                }
                if (North != null &&
                    North.East != null &&
                    North.East.isWall)
                {
                    wallSide |= Directions.NorthEast;
                }
                if (East != null &&
                    East.isWall)
                {
                    wallSide |= Directions.East;
                }
                if (South != null &&
                    South.East != null &&
                    South.East.isWall)
                {
                    wallSide |= Directions.SouthEast;
                }
                if (South != null &&
                    South.isWall)
                {
                    wallSide |= Directions.South;
                }
                if (South != null &&
                    South.West != null &&
                    South.West.isWall)
                {
                    wallSide |= Directions.SouthWest;
                }
                if (West != null &&
                    West.isWall)
                {
                    wallSide |= Directions.West;
                }
                if (North != null &&
                    North.West != null &&
                    North.West.isWall)
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
                    North.isWallBuilding)
                {
                    wallSide |= Directions.North;
                }
                if (North != null &&
                    North.East != null &&
                    North.East.isWallBuilding)
                {
                    wallSide |= Directions.NorthEast;
                }
                if (East != null &&
                    East.isWallBuilding)
                {
                    wallSide |= Directions.East;
                }
                if (South != null &&
                    South.East != null &&
                    South.East.isWallBuilding)
                {
                    wallSide |= Directions.SouthEast;
                }
                if (South != null &&
                    South.isWallBuilding)
                {
                    wallSide |= Directions.South;
                }
                if (South != null &&
                    South.West != null &&
                    South.West.isWallBuilding)
                {
                    wallSide |= Directions.SouthWest;
                }
                if (West != null &&
                    West.isWallBuilding)
                {
                    wallSide |= Directions.West;
                }
                if (North != null &&
                    North.West != null &&
                    North.West.isWallBuilding)
                {
                    wallSide |= Directions.NorthWest;
                }
                return wallSide;
            }
        }

        public MatPairStruct DesignationMat {
            get
            {
                MatPairStruct mat;
                switch (digDesignation)
                {
                    case TileDigDesignation.DEFAULT_DIG:
                        switch (tiletypeMaterial)
                        {
                            case TiletypeMaterial.PLANT:
                                mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.PlantGather);
                                break;
                            case TiletypeMaterial.TREE_MATERIAL:
                            case TiletypeMaterial.MUSHROOM:
                                mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.TreeCut);
                                break;
                            default:
                                mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.Default);
                                break;
                        }
                        break;
                    case TileDigDesignation.UP_DOWN_STAIR_DIG:
                        mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.UpDownStairs);
                        break;
                    case TileDigDesignation.CHANNEL_DIG:
                        mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.Channel);
                        break;
                    case TileDigDesignation.RAMP_DIG:
                        mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.Ramp);
                        break;
                    case TileDigDesignation.DOWN_STAIR_DIG:
                        mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.DownStairs);
                        break;
                    case TileDigDesignation.UP_STAIR_DIG:
                        mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.UpStairs);
                        break;
                    default:
                        if(Hidden)
                            mat = new MatPairStruct((int)MatBasic.DESIGNATION, (int)DesignationType.Hidden);
                        else
                            mat = new MatPairStruct(-1, -1);
                        break;
                }
                return mat;
            }
        }

        public bool GrowthAppliesEver(TreeGrowth growth)
        {
            if (special == TiletypeSpecial.DEAD
                || special == TiletypeSpecial.SMOOTH_DEAD)
            {
                return false;
            }
            switch (tiletypeMaterial)
            {
                case TiletypeMaterial.PLANT:
                    switch (shape)
                    {
                        case TiletypeShape.SAPLING:
                            if (!growth.sapling)
                                return false;
                            break;
                        case TiletypeShape.SHRUB:
                            //so far as I can understand, this is always on
                            break;
                        default:
                            return false;
                    }
                    break;
                case TiletypeMaterial.ROOT:
                    if (!growth.roots)
                        return false;
                    break;
                case TiletypeMaterial.TREE_MATERIAL:
                    switch (shape)
                    {
                        case TiletypeShape.WALL:
                        case TiletypeShape.RAMP:
                        case TiletypeShape.TRUNK_BRANCH:
                            if (!growth.trunk)
                                return false;
                            break;
                        case TiletypeShape.BRANCH:
                            if (special == TiletypeSpecial.SMOOTH)
                                return false;
                            if (direction == "--------")
                            {
                                if (!growth.light_branches)
                                    return false;
                            }
                            else if (!growth.heavy_branches)
                                return false;
                            break;
                        case TiletypeShape.TWIG:
                            if (!growth.twigs)
                                return false;
                            break;
                        default:
                            return false;
                    }
                    break;
                case TiletypeMaterial.MUSHROOM:
                    if (!growth.cap)
                        return false;
                    break;
                default:
                    return false;
            }

            if ((growth.trunk_height_start != -1 && growth.trunk_height_start > trunkPercent) || (growth.trunk_height_end != -1 && growth.trunk_height_end < trunkPercent))
                return false;

            return true;
        }

        public bool GrowthAppliesNow(TreeGrowth growth)
        {
            if (!GrowthAppliesEver(growth))
                return false;
            int currentTicks = TimeHolder.DisplayedTime.CurrentYearTicks;
            if ((growth.timing_start != -1 && growth.timing_start > currentTicks) || (growth.timing_end != -1 && growth.timing_end < currentTicks))
            {
                return false;
            }
            return true;

        }

        internal MatPairStruct GetMaterial(MeshLayer layer)
        {
            switch (layer)
            {
                case MeshLayer.StaticMaterial:
                case MeshLayer.StaticCutout:
                case MeshLayer.StaticTransparent:
                case MeshLayer.NaturalTerrain:
                    return material;
                case MeshLayer.BaseMaterial:
                case MeshLayer.BaseCutout:
                case MeshLayer.BaseTransparent:
                    return base_material;
                case MeshLayer.LayerMaterial:
                case MeshLayer.LayerCutout:
                case MeshLayer.LayerTransparent:
                    return layer_material;
                case MeshLayer.VeinMaterial:
                case MeshLayer.VeinCutout:
                case MeshLayer.VeinTransparent:
                    return vein_material;
                case MeshLayer.GrowthMaterial:
                case MeshLayer.GrowthCutout:
                case MeshLayer.GrowthTransparent:
                    if ((material.mat_type != 419)
                        || DFConnection.Instance.NetPlantRawList == null
                        || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= material.mat_index
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths.Count <= 0
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[0].mat == null)
                    {
                        return default(MatPairStruct);
                    }
                    return DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[0].mat;
                case MeshLayer.GrowthMaterial1:
                case MeshLayer.GrowthCutout1:
                case MeshLayer.GrowthTransparent1:
                    if ((material.mat_type != 419)
                        || DFConnection.Instance.NetPlantRawList == null
                        || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= material.mat_index
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths.Count <= 1
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[1].mat == null)
                    {
                        return default(MatPairStruct);
                    }
                    return DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[1].mat;
                case MeshLayer.GrowthMaterial2:
                case MeshLayer.GrowthCutout2:
                case MeshLayer.GrowthTransparent2:
                    if ((material.mat_type != 419)
                        || DFConnection.Instance.NetPlantRawList == null
                        || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= material.mat_index
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths.Count <= 2
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[2].mat == null)
                    {
                        return default(MatPairStruct);
                    }
                    return DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[2].mat;
                case MeshLayer.GrowthMaterial3:
                case MeshLayer.GrowthCutout3:
                case MeshLayer.GrowthTransparent3:
                    if ((material.mat_type != 419)
                        || DFConnection.Instance.NetPlantRawList == null
                        || DFConnection.Instance.NetPlantRawList.plant_raws.Count <= material.mat_index
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths.Count <= 3
                        || DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[3].mat == null)
                    {
                        return default(MatPairStruct);
                    }
                    return DFConnection.Instance.NetPlantRawList.plant_raws[material.mat_index].growths[3].mat;
                default:
                    return default(MatPairStruct);
            }
        }
    }
}
