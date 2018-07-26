using UnityEngine;

public class BodyPartChildPlaceholder : MonoBehaviour
{
    public string category;
    public bool categoryRegex;
    public string token;
    public bool tokenRegex;
    public enum PlacementCategory
    {
        Singular,
        ArrayStart,
        ArrayEnd
    }
    public PlacementCategory placement;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.02f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * 0.03f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * 0.03f);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 0.03f);
    }
}
