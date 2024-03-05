using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingDeliveryMethod : DeliveryMethod
{
    GameObject host = null;


    void OnTriggerEnter(Collider other)
    {
        if(host == null)
        {
            if(other.gameObject.GetComponent<PotionTarget>() != null || other.gameObject.transform.parent != null && other.gameObject.transform.parent.GetComponent<PotionTarget>() != null) //if the potion can stick to collision, it will
            {
                host = other.gameObject;
            }
        }
    }
    protected override void UniqueUpdate()
    {
        if (host != null)
        {
            transform.position = host.transform.position;
        }
    }
}
