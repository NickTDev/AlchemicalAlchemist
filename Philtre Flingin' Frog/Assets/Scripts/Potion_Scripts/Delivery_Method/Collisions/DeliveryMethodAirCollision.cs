using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryMethodAirCollision : MonoBehaviour
{
    [SerializeField] DeliveryMethod deliveryScript;
    [SerializeField] int groundLayer;
    bool underGround = false;

    void OnTriggerStay(Collider coll)
    {
        if (coll.gameObject.layer == groundLayer)
        {
            underGround = true;
        }
    }

    void Update()
    {
        if (deliveryScript.enabled)
        {
            deliveryScript.SetUnderGround(underGround);
        }
        underGround = false;
    }
}
