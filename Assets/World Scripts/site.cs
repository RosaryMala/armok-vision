using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class site : MonoBehaviour
{

    public int blocks_x = 1;
    public int blocks_y = 1;
    public int blocks_z = 1;
    int old_blocks_x = -1;
    int old_blocks_y = -1;
    int old_blocks_z = -1;

    int[] tiletypes;

    static int default_tile_type = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //this is called whenever a variable is changed from the editor.
    void OnValidate()
    {
        if (blocks_x < 1) blocks_x = 1;
        if (blocks_y < 1) blocks_y = 1;
        if (blocks_z < 1) blocks_z = 1;
        VerifyListSizes();
    }

    int CoordsToIndex(int inX, int inY, int inZ)
    {
        if (
            (inX < blocks_x * 16) &&
            (inY < blocks_y * 16) &&
            (inZ < blocks_z) &&
            (inX >= 0) &&
            (inY >= 0) &&
            (inZ >= 0)
            )
            return (inX + (inY * blocks_y * 16) + (inZ * blocks_x * 16 * blocks_y * 16));
        else
            return -1;
    }
    int OldCoordsToIndex(int inX, int inY, int inZ)
    {
        if (
            (inX < old_blocks_x * 16) &&
            (inY < old_blocks_y * 16) &&
            (inZ < old_blocks_z) &&
            (inX >= 0) &&
            (inY >= 0) &&
            (inZ >= 0)
            )
            return (inX + (inY * old_blocks_y * 16) + (inZ * old_blocks_x * 16 * old_blocks_y * 16));
        else
            return -1;
    }

    void VerifyListSizes()
    {
        if ((old_blocks_x != blocks_x) || (old_blocks_y != blocks_y) || (old_blocks_z != blocks_z))
        {
            int[] newTiletypes = new int[blocks_x * 16 * blocks_y * 16 * blocks_z];
            for (int cur_z = 0; cur_z < blocks_z; cur_z++)
                for (int cur_y = 0; cur_y < blocks_y * 16; cur_y++)
                    for (int cur_x = 0; cur_x < blocks_x * 16; cur_x++)
                    {
                        int newIndex = CoordsToIndex(cur_x, cur_y, cur_z);
                        int oldIndex = OldCoordsToIndex(cur_x, cur_y, cur_z);
                        if (oldIndex >= 0)
                            newTiletypes[newIndex] = tiletypes[oldIndex];
                        else
                            newTiletypes[newIndex] = default_tile_type;
                    }
            tiletypes = newTiletypes;
            old_blocks_x = blocks_x;
            old_blocks_y = blocks_y;
            old_blocks_z = blocks_z;
        }
    }
}
