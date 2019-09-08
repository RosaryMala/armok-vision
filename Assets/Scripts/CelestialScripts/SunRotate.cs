using UnityEngine;

public class SunRotate : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public float longitude;
    public float axialTilt;
    private int starMatrix;
    private int moonMatrix;

    public float GetLongitudeFromWorld(RemoteFortressReader.WorldMap world)
    {
        float tude = Mathf.InverseLerp(0, world.world_height, world.center_y);
        switch (world.world_poles)
        {
            case RemoteFortressReader.WorldPoles.NO_POLES:
                return 0;
            case RemoteFortressReader.WorldPoles.NORTH_POLE:
                tude = (1-tude) * 90;
                    break;
            case RemoteFortressReader.WorldPoles.SOUTH_POLE:
                tude = tude * 90;
                break;
            case RemoteFortressReader.WorldPoles.BOTH_POLES:
                tude = ((tude * 2) - 1) * -180;
                break;
            default:
                return 0;
        }
        return tude;
    }

    private void Awake()
    {
        starMatrix = Shader.PropertyToID("_StarRotationMatrix");
        moonMatrix = Shader.PropertyToID("_MoonRotationMatrix");
    }

    // Update is called once per frame
    void Update()
    {
        var seasonRotation = Quaternion.Euler((Mathf.Cos(TimeHolder.DisplayedTime.SolsticeAngle * Mathf.Deg2Rad) * axialTilt), 0, 0);
        var planetRotation = Quaternion.AngleAxis(TimeHolder.DisplayedTime.SunAngle, Vector3.back);
        var yearRotation = Quaternion.AngleAxis(TimeHolder.DisplayedTime.SolsticeAngle, Vector3.back) * Quaternion.Euler(0, 90, 0);
        var moonRotation = Quaternion.AngleAxis(-((TimeHolder.DisplayedTime.SolsticeAngle * 13) + 111.923076923076f), Vector3.back) * Quaternion.Euler(0, 90, 0);
        transform.rotation = planetRotation * seasonRotation * Quaternion.Euler(0, 90, 0);
        RenderSettings.skybox.SetMatrix(starMatrix, Matrix4x4.Rotate(planetRotation * yearRotation * seasonRotation));
        RenderSettings.skybox.SetMatrix(moonMatrix, Matrix4x4.Rotate(planetRotation * moonRotation * seasonRotation));
    }
}
