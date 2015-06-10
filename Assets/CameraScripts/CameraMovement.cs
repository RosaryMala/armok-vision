using UnityEngine;
using System.Collections;
using DFHack;

public class CameraMovement : MonoBehaviour {

    GameMap gameMap;
    public Transform cameraPos;
    bool following = true;
    public float speed = 10;

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
        
    }

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () {
        float moveZ = Input.GetAxis("CamUpDown");
        float moveX = Input.GetAxis("CamLeftRight");
        float moveY = Input.GetAxis("CamFrontBack");
        if(following)
        {
            transform.position = GameMap.DFtoUnityTileCenter(new DFCoord(gameMap.PosXTile, gameMap.PosYTile, gameMap.PosZ-1));
        }
        if(moveZ != 0.0f || moveY != 0.0f || moveX != 0.0f)
        {
            following = false;
            Vector3 movement = new Vector3(moveX, moveZ, moveY);
            transform.Translate(movement * Time.deltaTime * speed * cameraPos.localPosition.z * -1.0f, Space.Self);
            gameMap.UpdateCenter(transform.position);
        }
	}
}
