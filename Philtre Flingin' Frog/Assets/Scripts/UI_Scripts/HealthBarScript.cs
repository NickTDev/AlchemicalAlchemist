using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] bool faceCamera = true; //can be disabled ot tie the canvas's direction to the host rather than the camera
    GameObject myCamera;
    void Start()
    {
        myCamera = GameObject.FindWithTag("MainCamera");
    }
    void Update()
    {
        if(faceCamera)
        {
            transform.LookAt(myCamera.transform);
        }
    }
}
