using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] FMODUnity.EventReference interactSFX;
    public void Resume() //closes the current menu
    {
        Destroy(transform.parent.gameObject);
    }
    public void Exit() //closes the game
    {
        AudioEngineManager.PlaySound(interactSFX, 1f);
        Application.Quit();
    }

    public void ChangeScene()
    {
        AudioEngineManager.PlaySound(interactSFX, 1f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
