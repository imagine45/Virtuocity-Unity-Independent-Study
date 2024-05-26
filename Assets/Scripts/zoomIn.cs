using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class zoomIn : MonoBehaviour
{

    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private CinemachineComponentBase componentBase;
    float cameraDistance = 100;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zoom In"))
        {
            Debug.Log("Whats up");
            Debug.Log(virtualCamera.GetComponent<LensSettings>().FieldOfView);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zoom In"))
        {
            Debug.Log("Exited");
        }
    }
}
