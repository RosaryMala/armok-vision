﻿using RemoteFortressReader;
using System.Collections.Generic;
using UnityEngine;

namespace Building
{
    public class BuildingRoom : MonoBehaviour, IBuildingPart
    {
        public GameObject part;

        public List<GameObject> setParts = new List<GameObject>();
        public List<IBuildingPart> activeParts = new List<IBuildingPart>();

        public void UpdatePart(BuildingInstance buildingInput)
        {
            if (setParts.Count > 0)
            {
                foreach (var activePart in activeParts)
                {
                    activePart.UpdatePart(buildingInput);
                }
                return;
            }
            foreach (var item in setParts)
            {
                Destroy(item.gameObject);
            }
            setParts.Clear();
            activeParts.Clear();
            var room = buildingInput.room;
            if (room == null || room.extents.Count == 0)
            {
                for (int x = buildingInput.pos_x_min; x <= buildingInput.pos_x_max; x++)
                    for (int y = buildingInput.pos_y_min; y <= buildingInput.pos_y_max; y++)
                    {
                        GameObject newPart = Instantiate(part, GameMap.DFtoUnityCoord(x, y, buildingInput.pos_z_max), BuildingManager.TranslateDirection(buildingInput.direction), transform);
                        newPart.name += buildingInput.index;
                        setParts.Add(newPart);
                    }
            }
            else
            {
                for (int x = 0; x < room.width; x++)
                    for (int y = 0; y < room.height; y++)
                    {
                        var set = room.extents[x + y * room.width];
                        if (set == 0)
                            continue;
                        GameObject newPart = Instantiate(part, GameMap.DFtoUnityCoord(room.pos_x + x, room.pos_y + y, buildingInput.pos_z_max), BuildingManager.TranslateDirection(buildingInput.direction), transform);
                        newPart.name += buildingInput.index;
                        setParts.Add(newPart);
                    }
            }

            foreach (var setPart in setParts)
            {
                activeParts.AddRange(setPart.GetInterfacesInChildren<IBuildingPart>());
            }
            foreach (var activePart in activeParts)
            {
                activePart.UpdatePart(buildingInput);
            }
        }
    }
}
