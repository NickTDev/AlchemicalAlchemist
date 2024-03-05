using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMethodGroundCollision : MonoBehaviour
{
    [SerializeField] DeliveryMethod deliveryScript;
    [SerializeField] int groundLayer;
    [SerializeField] float verticalSpeed;
    bool onGround = false;

    void Start()
    {
        deliveryScript.SetVerticalSpeed(verticalSpeed);
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.layer == groundLayer)
        {
            onGround = true;
        }
    }

    void Update()
    {
        if(deliveryScript.enabled)
        {
            deliveryScript.SetOnGround(onGround);
        }
        onGround = false;
    }
}
