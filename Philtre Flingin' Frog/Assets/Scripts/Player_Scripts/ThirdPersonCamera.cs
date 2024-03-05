using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using Cinemachine;
using UnityEngine;
public class ThirdPersonCamera : MonoBehaviour
{
    Transform orientation; //SET IN CODE
    Transform playerObj; //SET IN CODE
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform player;
    //Transform player;
    public float rotationSpeed;
    public Vector3 pos;
    static Vector3 forward;
    void Start()
    {
        if (GameObject.FindWithTag("CM_FreeLook").GetComponent<ThirdPersonCamera>() != this) //destroys self if another one already present
        {
            Destroy(gameObject);
        }
        GameEvents.current.onPlayerRespawn += FindPlayer;

        FindPlayer(true);
        //starts as child of frog for ease of use, immediatly de-parents for decoupled movement
        transform.SetParent(null, false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FindPlayer(bool yes)
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = player.GetComponent<Rigidbody>();
        CinemachineFreeLook cinemachine = GetComponent<CinemachineFreeLook>();
        cinemachine.Follow = player;
        cinemachine.LookAt = player;

        foreach (Transform child in player.transform)
        {
            if (child.name == "Orientation")
            {
                orientation = child;
            }
            else if (child.name == "PlayerModel")
            {
                playerObj = child;
            }
        }
    }
    private void Update()
    {
        if(player == null) //refuses to work anywhere other than update for some reason
        {
            FindPlayer(true);
        }
        if (!GameManager.GetPaused())
        {
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;
            playerObj.eulerAngles = new Vector3(orientation.eulerAngles.x, orientation.eulerAngles.y, playerObj.eulerAngles.z);
        }
    }
}