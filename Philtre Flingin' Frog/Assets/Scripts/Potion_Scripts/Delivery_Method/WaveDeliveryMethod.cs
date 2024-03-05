using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveDeliveryMethod : DeliveryMethod
{
    [SerializeField] float moveSpeed;

    protected override void UniqueStart()
    {
        transform.Rotate(0.0f, -90.0f, 90.0f, Space.Self);
    }
    protected override void UniqueUpdate()
    {
        Move();
    }
    void Move()
    {
        if (!GameManager.GetPaused())
        {
            transform.Translate(-transform.right * moveSpeed * Time.deltaTime);
        }
    }
}
