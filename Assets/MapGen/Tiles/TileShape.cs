using UnityEngine;
using System.Collections;
using RemoteFortressReader;

public class TileShape : GenericTile
{
    public GenericTile noShape;
    public GenericTile empty;
    public GenericTile floor;
    public GenericTile boulder;
    public GenericTile pebbles;
    public GenericTile wall;
    public GenericTile fortification;
    public GenericTile stairUp;
    public GenericTile stairDown;
    public GenericTile stairUpDown;
    public GenericTile ramp;
    public GenericTile rampTop;
    public GenericTile brookBed;
    public GenericTile brookTop;
    public GenericTile treeShape;
    public GenericTile sapling;
    public GenericTile shrub;
    public GenericTile endlessPit;
    public GenericTile branch;
    public GenericTile trunkBranch;
    public GenericTile twig;

    public override Mesh GetMesh(GameMap map, int x, int y, int z)
    {
        MapTile tile = MapDataStore.GetTile(x, y, z);
        if (tile == null) return null;
        TiletypeShape shape = DFConnection.Instance.NetTiletypeList.tiletype_list[tile.tileType].shape;
        switch (shape)
        {
            case TiletypeShape.NO_SHAPE:
                if (noShape == null) return null;
                return noShape.GetMesh(map, x, y, z);
            case TiletypeShape.EMPTY:
                if (empty == null) return null;
                return empty.GetMesh(map, x, y, z);
            case TiletypeShape.FLOOR:
                if (floor == null) return null;
                return floor.GetMesh(map, x, y, z);
            case TiletypeShape.BOULDER:
                if (boulder == null) return null;
                return boulder.GetMesh(map, x, y, z);
            case TiletypeShape.PEBBLES:
                if (pebbles == null) return null;
                return pebbles.GetMesh(map, x, y, z);
            case TiletypeShape.WALL:
                if (wall == null) return null;
                return wall.GetMesh(map, x, y, z);
            case TiletypeShape.FORTIFICATION:
                if (fortification == null) return null;
                return fortification.GetMesh(map, x, y, z);
            case TiletypeShape.STAIR_UP:
                if (stairUp == null) return null;
                return stairUp.GetMesh(map, x, y, z);
            case TiletypeShape.STAIR_DOWN:
                if (stairDown == null) return null;
                return stairDown.GetMesh(map, x, y, z);
            case TiletypeShape.STAIR_UPDOWN:
                if (stairUpDown == null) return null;
                return stairUpDown.GetMesh(map, x, y, z);
            case TiletypeShape.RAMP:
                if (ramp == null) return null;
                return ramp.GetMesh(map, x, y, z);
            case TiletypeShape.RAMP_TOP:
                if (rampTop == null) return null;
                return rampTop.GetMesh(map, x, y, z);
            case TiletypeShape.BROOK_BED:
                if (brookBed == null) return null;
                return brookBed.GetMesh(map, x, y, z);
            case TiletypeShape.BROOK_TOP:
                if (brookTop == null) return null;
                return brookTop.GetMesh(map, x, y, z);
            case TiletypeShape.TREE_SHAPE:
                if (treeShape == null) return null;
                return treeShape.GetMesh(map, x, y, z);
            case TiletypeShape.SAPLING:
                if (sapling == null) return null;
                return sapling.GetMesh(map, x, y, z);
            case TiletypeShape.SHRUB:
                if (shrub == null) return null;
                return shrub.GetMesh(map, x, y, z);
            case TiletypeShape.ENDLESS_PIT:
                if (endlessPit == null) return null;
                return endlessPit.GetMesh(map, x, y, z);
            case TiletypeShape.BRANCH:
                if (branch == null) return null;
                return branch.GetMesh(map, x, y, z);
            case TiletypeShape.TRUNK_BRANCH:
                if (trunkBranch == null) return null;
                return trunkBranch.GetMesh(map, x, y, z);
            case TiletypeShape.TWIG:
                if (twig == null) return null;
                return twig.GetMesh(map, x, y, z);
            default:
                if (noShape == null) return null;
                return noShape.GetMesh(map, x, y, z);
        }
    }
}
