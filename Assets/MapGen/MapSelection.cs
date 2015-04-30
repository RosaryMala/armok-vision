using UnityEngine;
using System.Collections;

public class MapSelection : MonoBehaviour {

    public GameMap targetMap;
    Ray mouseRay;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
        //
        //for (float i = 0; i < 100; i++) {
        //
        //}
	}

    void OnDrawGizmos () {
        for (float t = 30; t < 100; t++) {
            Gizmos.color = Color.red;
            Vector3 loc = mouseRay.origin + mouseRay.direction * t;
            Vector3 adjusted = GameMap.DFtoUnityTileCenter(GameMap.UnityToDFCoord(loc));
            Gizmos.DrawSphere (loc, .1f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine (loc, adjusted);
            Gizmos.color = Color.white;
            Gizmos.DrawSphere (adjusted, .1f);
        }
    }
}
