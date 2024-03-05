using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;

public class AttackProjection : MonoBehaviour
{
    public GameObject projectile;
    public Transform orientation;
    public Transform launchPoint;
    Projectile proj;
    LineRenderer lineRenderer;

    public GameObject player;
    AttackScript attack;

    public int numPoints = 50;
    public float timeBetweenPoints = .1f;
    public LayerMask collidableLayers;
    public Transform landingSpot;
    RaycastHit hit;

    void Start()
    {
        proj = projectile.GetComponent<Projectile>();
        lineRenderer = GetComponent<LineRenderer>();
        attack = player.GetComponent<AttackScript>();
        landingSpot.position = new Vector3(1000, 1000, 1000);
    }


    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !GameManager.GetPaused())
        {
            lineRenderer.positionCount = numPoints;
            List<Vector3> points = new List<Vector3>();
            Vector3 startingPosition = launchPoint.position;
            Vector3 startingVelo = new Vector3(orientation.transform.forward.x * proj.speed * attack.timeHeldDown / attack.maxHeldDown, proj.yForce * attack.timeHeldDown / attack.maxHeldDown, orientation.transform.forward.z * proj.speed * attack.timeHeldDown / attack.maxHeldDown);
            startingVelo.x *= 10f;
            startingVelo.z *= 10f;
            for (float i = 0f; i < lineRenderer.positionCount; i += timeBetweenPoints)
            {
                Vector3 newPoint = startingPosition + i * startingVelo;
                newPoint.y = startingPosition.y + startingVelo.y * i + proj.gravity * Time.deltaTime / 2f * i * i;
                points.Add(newPoint);
                if (Physics.OverlapSphere(newPoint, .1f, collidableLayers).Length > 0)
                {
                    lineRenderer.positionCount = points.Count;
                    break;
                }
            }
            landingSpot.position = points.ToArray()[points.Count - 1];
            if (Physics.Raycast(landingSpot.position, Vector3.down,out hit))
            {
                Quaternion rotation = Quaternion.LookRotation(-hit.normal);
                rotation.x = rotation.x + 90f;
            }
            lineRenderer.SetPositions(points.ToArray());
        }
        else
        {
            lineRenderer.positionCount = 0;
            landingSpot.position = new Vector3(1000,1000,1000);
        }

    }
}
