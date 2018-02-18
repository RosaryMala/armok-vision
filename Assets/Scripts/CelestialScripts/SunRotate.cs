using UnityEngine;

public class SunRotate : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    public float longitude;
    public float axialTilt;

    public Transform longitudeAxis;
    public Transform seasonAxis;

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

    // Update is called once per frame
    void Update()
    {
        Quaternion rotate = Quaternion.AngleAxis(TimeHolder.DisplayedTime.SunAngle, Vector3.back);
        transform.localRotation = rotate;
        seasonAxis.localRotation = Quaternion.Euler((Mathf.Cos(TimeHolder.DisplayedTime.SolsticeAngle * Mathf.Deg2Rad) * axialTilt) + 90, 0, 0);
    }
}
