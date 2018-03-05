using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationManager : MonoBehaviour
{
    public static DecorationManager Instance { get; private set; }

    public GameObject Image;
    public GameObject Ring;
    public GameObject Spike;
    [SerializeField]
    private GameObject _shape;

    Dictionary<int, GameObject> shapes = new Dictionary<int, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ContentLoader.RegisterLoadCallback(LoadShapes);
    }

    private IEnumerator LoadShapes()
    {
        if (DFConnection.Instance.NetLanguageList == null)
            yield break;

        var shapeList = DFConnection.Instance.NetLanguageList.shapes;
        if (shapeList == null)
            yield break;

        var stopWatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < shapeList.Count; i++)
        {
            var shape = shapeList[i];

            string path = "Shapes/" + shape.id;

            var loadedItem = Resources.Load<GameObject>(path);
            if (loadedItem == null)
            {
                if (stopWatch.ElapsedMilliseconds > 100)
                {
                    yield return null;
                    stopWatch.Reset();
                    stopWatch.Start();
                }
                continue;
            }

            shapes[i] = loadedItem;

            if (stopWatch.ElapsedMilliseconds > 100)
            {
                yield return null;
                stopWatch.Reset();
                stopWatch.Start();
            }
        }
    }

    public GameObject GetShape(int shapeIndex)
    {
        if (shapeIndex == -1)
            return _shape;
        if (shapes.ContainsKey(shapeIndex))
            return shapes[shapeIndex];
        if (DFConnection.Instance.NetLanguageList != null && shapeIndex >= 0 && shapeIndex < DFConnection.Instance.NetLanguageList.shapes.Count)
            Debug.LogWarning("No model defined for shape: " + DFConnection.Instance.NetLanguageList.shapes[shapeIndex].id);
        else
            Debug.LogWarning("Unknown shape: " + shapeIndex);

        return _shape;
    }
}
