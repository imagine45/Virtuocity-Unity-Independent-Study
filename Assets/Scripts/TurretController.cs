using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{

    public GameObject player;
    public Transform viewPoint;
    public Animator animator;
    public ParticleSystem bullet;

    private float distance;
    private float rotationSpeed = 6f;
    float angle;

    private int maxDist = 15;
    private int maxAngle = 120;

    private float shootTimer = 0;
    private int shootTime = 1;

    void FixedUpdate()
    {

        //print(shootTimer);

        distance = Vector3.Distance(player.transform.position, viewPoint.position);
        angle = (player.transform.position.x - viewPoint.position.x > 0) ? 
            -Mathf.Acos((player.transform.position.y - viewPoint.position.y) / distance) * Mathf.Rad2Deg : 
            Mathf.Acos((player.transform.position.y - viewPoint.position.y) / distance) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle % 360));

        distance = Vector3.Distance(player.transform.position, viewPoint.position);

        if (canTrack())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        if (canShoot())
        {
            shoot();
        }

        //print("Shoot timer: " + shootTimer);
    }

    private bool canTrack()
    {
        RaycastHit2D hit;

        int layerMask = ~(1 << 8);
        hit = Physics2D.Raycast(viewPoint.position, player.transform.position - transform.position, maxDist, layerMask);

        if (hit && hit.collider.tag != "Player") 
            return false;

        if (distance <= maxDist && Mathf.Abs(angle) <=maxAngle)
        {
            return true;
        }
        return false;
    }

    private void shoot()
    {
        animator.SetInteger("Charge", 0);
        bullet.Play();
        shootTimer = 0;
        AudioManager.instance.PlayOneShot(FMODEvents.instance.turretShoot, transform.position);
    }

    private bool canShoot()
    {
        if (canTrack() && shootTimer >= shootTime)
        {
            return true;
        }
        else if (canTrack())
        {
            shootTimer += Time.deltaTime;
        } else
        {
            shootTimer = (shootTimer <= 0) ? 0 : shootTimer - Time.deltaTime;
        }

        animator.SetInteger("Charge", (int)(shootTimer * 4));

        return false;
    }
}
