using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Update is called once per frame
    public void changeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
