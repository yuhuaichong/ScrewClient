using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class ScreenAdpar : MonoBehaviour
{
    [Tooltip("Camera used to convert between screen and world space. Defaults to Camera.main.")]
    public Camera targetCamera;

    [Tooltip("Pixels from the top edge of the screen.")]
    public float topMarginPixels = 0f;

    [Tooltip("If true, updates every frame to adapt to resolution/aspect changes.")]
    public bool updateEveryFrame = true;

    void OnEnable()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        //Apply();
    }
    // IEnumerator Start()
    // {
    //     yield return new WaitForSeconds(1f);
    //     Apply();
    // }

    // void Update()
    // {
    //     if (!updateEveryFrame)
    //     {
    //         return;
    //     }
    //     Apply();
    // }

    private void Apply()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                return;
            }
        }

        Vector3 worldPos = transform.position;
        float zDistance = Mathf.Abs(worldPos.z - targetCamera.transform.position.z);

        Vector3 screenPos = targetCamera.WorldToScreenPoint(worldPos);
        screenPos.y = Screen.height - topMarginPixels;
        screenPos.z = zDistance;

        Vector3 worldAtTop = targetCamera.ScreenToWorldPoint(screenPos);
        transform.position = new Vector3(worldPos.x, worldAtTop.y, worldPos.z);
    }
}
