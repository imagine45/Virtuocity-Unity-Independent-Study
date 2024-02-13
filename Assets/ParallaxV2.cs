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
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
