using UnityEngine;

[CreateAssetMenu]
public class ClothingTexture : ScriptableObject
{
    public bool isDress;
    public Texture2D texture;
    public Texture2D mask;
    public Texture2D bump;
}
