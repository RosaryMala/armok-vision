using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class FadeBelowHorizon : MonoBehaviour {

    public float angleRange = 1;

    Color originalColor;
    MeshRenderer meshRenderer;

    public Transform Sun;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start () {
        originalColor = meshRenderer.material.GetColor("_Color");
    }
	
	// Update is called once per frame
	void Update () {
        float x = 90 - Vector3.Angle(Sun.transform.forward, Vector3.up);

        if (x > 180)
            x -= 360;

        x += (angleRange / 2);
        x /= angleRange;
        x = Mathf.Clamp(x, 0, 1);
        //Debug.Log(this.name + " light=" + x + ", xrot=" + transform.rotation.eulerAngles.x);
        meshRenderer.material.SetColor("_Color", originalColor * x);

    }
}
