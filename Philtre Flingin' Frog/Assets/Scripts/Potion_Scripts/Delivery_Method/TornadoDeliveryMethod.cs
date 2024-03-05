using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoDeliveryMethod : DeliveryMethod
{
    [SerializeField] float moveSpeed;
    [SerializeField] float minDirectionTime; //the minimum time to move in the same direction
    [SerializeField] float maxDirectionTime; //the maximum time to move in the same direction
    [SerializeField] float minRotation; //the minimum amount that it must turn each time it recalculates

    float directionTimer = 0.0f; //how long it's been moving in this direction
    float currentTimeForDirection = -1.0f; //how long it should move in this direction

    protected override void UniqueStart() //they start out in set orders each time
    {
        transform.Rotate(0.0f, orderSpawned * 120f, 0.0f, Space.Self); //determines new direction
        currentTimeForDirection = Random.Range(minDirectionTime, maxDirectionTime); //determines how long it will move in this direction
        directionTimer = 0.0f;
    }
    protected override void UniqueUpdate()
    {
        if(!GameManager.GetPaused())
        {
            directionTimer += Time.deltaTime;
            if (directionTimer >= currentTimeForDirection)
            {
                CalculateDirection();
            }
            Move();
        }
    }

    void Move()
    {
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime);
    }

    void CalculateDirection()
    {
        transform.Rotate(0.0f, Random.Range(minRotation, 360-minRotation), 0.0f, Space.Self); //determines new direction
        currentTimeForDirection = Random.Range(minDirectionTime, maxDirectionTime); //determines how long it will move in this direction
        directionTimer = 0.0f;
    }
}
