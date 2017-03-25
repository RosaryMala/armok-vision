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
        float tude = Mathf.InverseLerp(0, world.WorldHeight, world.CenterY);
        switch (world.WorldPoles)
        {
            case RemoteFortressReader.WorldPoles.NoPoles:
                return 0;
            case RemoteFortressReader.WorldPoles.NorthPole:
                tude = (1-tude) * 90;
                    break;
            case RemoteFortressReader.WorldPoles.SouthPole:
                tude = tude * 90;
                break;
            case RemoteFortressReader.WorldPoles.BothPoles:
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
