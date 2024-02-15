using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{

    public GameObject player;
    public Transform viewPoint;

    private float distance;
    private float rotationSpeed = 6f;
    float angle;

    private int maxDist = 15;
    private int maxAngle = 90;

    void FixedUpdate()
    {
        angle = Mathf.Atan2(player.transform.position.y - viewPoint.position.y, player.transform.position.x - viewPoint.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, (angle - 90) % 360));

        distance = Vector3.Distance(player.transform.position, viewPoint.position);

        if (canTrack())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        //print((angle) % 360);
    }

    private bool canTrack()
    {
        RaycastHit2D hit;

        int layerMask = ~(1 << 8);
        hit = Physics2D.Raycast(viewPoint.position, player.transform.position - transform.position, maxDist, layerMask);

        if (hit && hit.collider.tag != "Player") 
            return false;

        //print("Can see player");

        if (distance <= maxDist && angle >= 90 - maxAngle && angle <= 90 + maxAngle)
        {
            return true;
        }
        return false;
    }
}
