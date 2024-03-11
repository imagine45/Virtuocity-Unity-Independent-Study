using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ParallaxV2 : MonoBehaviour
{
    public float parallaxMult;

    private Vector2 startPos;
    private PixelPerfectCamera pixelPerfect;
    private enum axisTracking { X = 0, Y = 1, X_AND_Y = 2}
    [SerializeField] axisTracking axis;

    public float yDamping = 1f;
    public float xDamping = 1f;
    private void Start()
    {
        pixelPerfect = Camera.main.GetComponent<PixelPerfectCamera>();
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCamUpdate);
        startPos = pixelPerfect.RoundToPixel(transform.position);
    }

    private void OnCamUpdate(CinemachineBrain arg0)
    {
        Vector2 cameraOffset = pixelPerfect.RoundToPixel(arg0.transform.position);
        Vector2 pos = pixelPerfect.RoundToPixel(startPos + cameraOffset * parallaxMult);
        transform.position = new Vector3((axis == axisTracking.X || axis == axisTracking.X_AND_Y) ? pos.x * xDamping: transform.position.x,
            (axis == axisTracking.Y || axis == axisTracking.X_AND_Y) ? pos.y * yDamping: transform.position.y, transform.position.z);
    }
}
