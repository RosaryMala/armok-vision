using UnityEngine;
using System.Collections;

public class GameWindow : MonoBehaviour
{
    public int viewWidth = 80;
    public int viewHeight = 25;
    public int viewDist = 5;
    public float nearClipOffset = 0;
    float viewPortWidth = 80*MapBlock.tileWidth;
    float viewPortHeight = 25*MapBlock.tileWidth;
    public float verticalShift;
    float viewDistance = 5*MapBlock.tileHeight;
    float n, f, r, l, t, b;
    Matrix4x4 mat = new Matrix4x4();
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        viewPortWidth = viewWidth * MapBlock.tileWidth;
        if(false)
            viewPortHeight = viewHeight * MapBlock.tileWidth;
        else
            viewPortHeight = viewPortWidth * (float)Screen.height / (float)Screen.width;
        viewDistance = viewDist * MapBlock.tileHeight * 10;
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float viewportAspect = (float)viewPortWidth / (float)viewPortHeight;

        float fraction = screenAspect / viewportAspect;

        float height = (viewPortHeight / 2.0f) / Mathf.Sin((GetComponent<Camera>().fieldOfView / 2) * Mathf.Deg2Rad);
        transform.localPosition = new Vector3(0, height, -verticalShift);

        GetComponent<Camera>().nearClipPlane = height - nearClipOffset;
        GetComponent<Camera>().farClipPlane = height + viewDistance;
        RenderSettings.fogStartDistance = GetComponent<Camera>().nearClipPlane;
        RenderSettings.fogEndDistance = GetComponent<Camera>().farClipPlane;
        if (fraction > 1.0f) //If the screen is wider than the DF Viewport
        {
            GetComponent<Camera>().rect = new Rect(0.5f - ((1.0f / fraction) / 2.0f), 0.0f, 1.0f / fraction, 1.0f);
        }
        else //If the DF Viewport is wider.
        {
            GetComponent<Camera>().rect = new Rect(0.0f, 0.5f - (fraction / 2.0f), 1.0f, fraction);
        }
        //make a custom camera matrix, rather than use the inbuilt one.
        n = GetComponent<Camera>().nearClipPlane;
        f = GetComponent<Camera>().farClipPlane;
        r = viewPortWidth / 2.0f;
        l = -viewPortWidth / 2.0f;
        t = verticalShift + (viewPortHeight / 2.0f);
        b = verticalShift - (viewPortHeight / 2.0f);


        mat[0, 0] = 2 * n / (r - l); mat[1, 0] = 0; mat[2, 0] = 0; mat[3, 0] = 0;
        mat[0, 1] = 0; mat[1, 1] = 2 * n / (t - b); mat[2, 1] = 0; mat[3, 1] = 0;
        mat[0, 2] = (r + l) / (r - l); mat[1, 2] = (t + b) / (t - b); mat[2, 2] = -(f + n) / (f - n); mat[3, 2] = -1;
        mat[0, 3] = 0; mat[1, 3] = 0; mat[2, 3] = -2 * f * n / (f - n); ; mat[3, 3] = 0;

        GetComponent<Camera>().projectionMatrix = mat;
    }
}
