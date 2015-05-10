using UnityEngine;
using System.Collections;
using DFHack;

// Singleton that holds map data.
public static class MapDataStore {
    public static MapTile[,,] Tiles { get; private set; }
    public static DFCoord Size { get; private set; }

    // Called from InitStatics in DFConnection
    public static void Init (int x, int y, int z) {
        if (Size != default(DFCoord) || Tiles != null) {
            throw new UnityException("MapDataStore already initialized!");
        }
        Size = new DFCoord(x, y, z);
        Tiles = new MapTile[x, y, z];
    }

    public static void Uninit () {
        Tiles = null;
        Size = default(DFCoord);
    }

    public static void Clear () {
        foreach (var tile in Tiles) {
            if (tile != null) {
                tile.Reset();
            }
        }
    }

    public static bool InBounds(int x, int y, int z) {
        return 0 <= x && x < Size.x &&
                0 <= y && y < Size.y &&
                0 <= z && z < Size.z;
    }

    public static bool InBounds(DFCoord coord) {
        return InBounds(coord.x, coord.y, coord.z);
    }

    // Use these accessors when you want out-of-bounds to just return null
    public static MapTile GetTile (int x, int y, int z)
    {
        if (InBounds (x, y, z)) {
            return Tiles [x, y, z];
        } else {
            return null;
        }
    }
    public static MapTile GetTile (DFCoord coord) {
        return GetTile (coord.x, coord.y, coord.z);
    }

    // Add an initialized tile
    public static void AddTile (MapTile tile) {
        if (tile == null) throw new UnityException("Can't add null without position!");
        Tiles[tile.position.x, tile.position.y, tile.position.z] = tile;
    }

    // Return the tile at this location, initializing if necessary.
    public static MapTile GetOrInitTile (int x, int y, int z) {
        if (InBounds(x, y, z)) {
            MapTile tile = Tiles[x, y, z];
            if (tile == null) {
                tile = new MapTile(new DFCoord(x, y, z));
                Tiles[x, y, z] = tile;
            }
            return tile;
        } else {
            throw new UnityException("Can't init out of bounds tile");
        }
    }
}
