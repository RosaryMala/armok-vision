using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stacker : MonoBehaviour {
    public enum StackType
    {
        Straight,
        FermatSpiral,
        SpiralHemisphere
    }

    static List<Vector3> pointList = new List<Vector3>();
    static float minDistance = float.MaxValue;
    static int minIndex = 0;
    static List<int> levelCount = new List<int>();


    public StackType stackType;

    public int numItems = 1;

    static readonly float verticalDistance = 0.5f;
    static readonly float horizontalDistance = 0.5f;

    public float sphereRadius = 0.5f;

    public float maxRadius = 0;

    private void OnDrawGizmos()
    {
        maxRadius = 0;
        for (int i = 0; i < numItems; i++)
        {
            Vector3 pos = Vector3.zero;
            switch (stackType)
            {
                case StackType.Straight:
                    pos = StraightStack(i);
                    maxRadius = Mathf.Max(maxRadius, pos.magnitude);
                    break;
                case StackType.FermatSpiral:
                    pos = FermatSpiral(i);
                    maxRadius = Mathf.Max(maxRadius, pos.magnitude);
                    break;
                case StackType.SpiralHemisphere:
                    pos = SpiralHemisphere(i);
                    maxRadius = Mathf.Max(maxRadius, pos.magnitude);
                    break;
                default:
                    break;
            }
            Gizmos.DrawSphere(transform.TransformPoint(pos), sphereRadius);
        }
    }

    public static Vector3 StraightStack(int num)
    {
        return new Vector3(0, num * verticalDistance, 0);
    }

    public static Vector3 FermatSpiral(int num)
    {
        if (num == 0)
            return Vector3.zero;
        return Quaternion.Euler(0, 137.508f * num, 0) * new Vector3(horizontalDistance * Mathf.Sqrt(num), 0, 0);
    }

    public static Vector3 SpiralHemisphere(int num)
    {
        for (int i = pointList.Count; i <= num; i++)
        {
            float minDistance = float.MaxValue;
            int minIndex = 0;
            for (int j = 0; j <= levelCount.Count; j++)
            {
                int levelIndex = 0;
                if (j < levelCount.Count)
                    levelIndex = levelCount[j];
                Vector3 pos = FermatSpiral(levelIndex);
                pos += new Vector3(0, j * verticalDistance, 0);
                if (pos.sqrMagnitude < minDistance)
                {
                    minIndex = j;
                    minDistance = pos.sqrMagnitude;
                }
            }
            if (minIndex == levelCount.Count)
                levelCount.Add(1);
            else
                levelCount[minIndex]++;
            pointList.Add(Quaternion.Euler(0, minIndex * 137.508f, 0) * (FermatSpiral(levelCount[minIndex] - 1) + new Vector3(0, minIndex * verticalDistance, 0)));
        }
        return pointList[num];
    }
}
