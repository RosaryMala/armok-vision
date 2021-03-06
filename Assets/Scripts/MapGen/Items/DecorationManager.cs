﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649 //Variable not assigned.

[ExecuteInEditMode]
public class DecorationManager : MonoBehaviour
{
    static DecorationManager _instance;
    public static DecorationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<DecorationManager>();
            if (_instance == null)
                _instance = Instantiate(Resources.Load<DecorationManager>("DecorationManager"));
            return _instance;
        }
    }

    public GameObject Image;
    public GameObject Ring;
    public GameObject Spike;
    [SerializeField]
    private GameObject _shape;
    [SerializeField]
    private ProgressBar mainProgressBar;
    [SerializeField]
    private ProgressBar subProgressBar;

    Dictionary<int, GameObject> shapes = new Dictionary<int, GameObject>();

    private void Awake()
    {
        if (_instance != null)
        {
            if (_instance != this)
                Destroy(this);
        }
        else
        {
            _instance = this;
        }

        if (!Application.isPlaying)
        {
            var loader = LoadShapes();
            while (loader.MoveNext())
            {

            }
            Debug.Log("Loaded shapes.");
        }
        else ContentLoader.RegisterLoadCallback(LoadShapes);
    }

    private IEnumerator LoadShapes()
    {
        if (DFConnection.Instance == null)
            yield break;
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
                if (stopWatch.ElapsedMilliseconds > ContentLoader.LoadFrameTimeout)
                {
                    yield return null;
                    stopWatch.Reset();
                    stopWatch.Start();
                }
                continue;
            }

            shapes[i] = loadedItem;

            if (stopWatch.ElapsedMilliseconds > ContentLoader.LoadFrameTimeout)
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
        if (DFConnection.Instance == null)
            return _shape;
        if (DFConnection.Instance.NetLanguageList != null && shapeIndex >= 0 && shapeIndex < DFConnection.Instance.NetLanguageList.shapes.Count)
            Debug.LogWarning("No model defined for shape: " + DFConnection.Instance.NetLanguageList.shapes[shapeIndex].id);
        else
            Debug.LogWarning("Unknown shape: " + shapeIndex);

        return _shape;
    }
}
