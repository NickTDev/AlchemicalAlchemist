using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    void Start()
    {
        GameManager.SetPaused(true);
    }
    void OnDestroy()
    {
        GameManager.SetPaused(false);
    }
}
