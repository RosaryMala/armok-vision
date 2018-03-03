using UnityEngine;
using System.Collections;

public class CollisionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.End))
        {
           MapDataStore.Main.CheckCollision(transform.position);
        }
	}
}
