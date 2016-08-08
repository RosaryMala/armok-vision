using UnityEngine;

// fixes the deferred lighting missing final copy&resolve, so the next camera gets the correctly final processed image in the temp screen RT as input
// NOTE: The script must be the last in the image effect chain, so order it in the inspector!
[ExecuteInEditMode]
public class CopyToScreenRT : MonoBehaviour
{
    private RenderTexture activeRT; // hold the org. screen RT

    private Camera cameraComp;

    private void OnPostRender()
    {
        if (cameraComp.actualRenderingPath == RenderingPath.DeferredShading)
        {
            activeRT = RenderTexture.active;
        }
        else
        {
            activeRT = null;
        }
    }

    public void Awake()
    {
        cameraComp = GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (cameraComp.actualRenderingPath == RenderingPath.DeferredShading && activeRT)
        {
            if (src.format == activeRT.format)
            {
                Graphics.Blit(src, activeRT);
            }
            else
            {
                Debug.LogWarning(string.Format("Cant resolve texture, because of different formats! ({0}, {1})", src.format, activeRT.format));
            }
        }

        // script must be last anyway, so we don't need a final copy?
        Graphics.Blit(src, dest); // just in case we are not last!
    }
}