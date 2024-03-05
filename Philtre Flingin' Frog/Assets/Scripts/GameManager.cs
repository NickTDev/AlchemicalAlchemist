using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    static bool isPaused = false;
    [SerializeField] GameObject hotkeyUIPrefab;
    [SerializeField] GameObject eventListener;
    [SerializeField] GameObject pauseMenuPrefab;
    [SerializeField] GameObject deathScreenPrefab;
    GameObject pauseMenu = null;
    GameObject player;
    static GameObject hotkeyBar = null;
    static GameObject healthBar = null;

    GameObject upgradeMenu;

    float escTimer = 0;
    float escGracePeriod = 0.5f;

    static CinemachineFreeLook freeRotation; //cinemachine

    public Checkpoint currentCheckpoint;
    public bool firstPlayer = true;

    public GameObject cursorCanvasPrefab;
    static GameObject cursorCanvas;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(instance);
        }
        /*
        else
        {
            Destroy(this.gameObject);
        }
        */

        FindPlayer();
        if (GameObject.FindWithTag("GameManager").GetComponent<GameManager>() != this) //destroys self if another gamemanager already present
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
        //spawns objects that every scene needs
        SpawnObjects();
        freeRotation = FindObjectOfType<CinemachineFreeLook>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        GameEvents.current.onPlayerRespawn += PlayerRespawn;
    }

    void PlayerRespawn(bool yes = true)
    {
        FindPlayer();
        Instantiate(deathScreenPrefab, transform.position, transform.rotation); //spawns death screen
    }
    void FindPlayer()
    {
        player = GameObject.FindWithTag("Player");
    }

    public static GameObject GetPlayer()
    {
        return instance.player;
    }

    void Update()
    {
        Cursor.visible = false;
        escTimer -= Time.deltaTime;
        if (Input.GetKey("escape") && escTimer < 0.0f)
        {
            escTimer = escGracePeriod;
            if (PlayerInventory.inventoryUIOpen) //closes brewing menu if open
            {
                player.GetComponent<PlayerInventory>().CloseUI();
            }
            else if (pauseMenu == null && !isPaused) //opens pause menu if no brewing UI or pause menu
            {
                pauseMenu = Instantiate(pauseMenuPrefab, transform.position, transform.rotation) as GameObject;
            }
            else if (pauseMenu != null) //closes pause menu if open
            {
                Destroy(pauseMenu);
            }
        }
        if(player == null)
        {
            FindPlayer();
        }
        /*
        if(false)//Input.GetKeyDown(KeyCode.LeftShift))
        {
            if(upgradeMenu == null)
            {
                upgradeMenu = Instantiate(upgradeMenuPrefab, transform.position, transform.rotation) as GameObject;
            }
            else
            {
                Destroy(upgradeMenu);
            }
        }
        */
    }

    public static bool GetPaused()
    {
        return isPaused;
    }
    public static void SetPaused(bool pause)
    {
        isPaused = pause;
        Cursor.visible = pause;
        freeRotation = FindObjectOfType<CinemachineFreeLook>();
        if (freeRotation != null) //loves to be null for some reason
        {
            if (pause)
            {
                cursorCanvas = Instantiate(GameObject.FindWithTag("GameManager").GetComponent<GameManager>().cursorCanvasPrefab, GameObject.FindWithTag("GameManager").transform.position, GameObject.FindWithTag("GameManager").transform.rotation) as GameObject;
                freeRotation.m_YAxis.m_InputAxisName = "NULL_AXIS";
                freeRotation.m_XAxis.m_InputAxisName = "NULL_AXIS";
                //makes UI invisible when paused
                hotkeyBar = GameObject.FindWithTag("Hotkey_Bar");
                healthBar = GameObject.FindWithTag("Player_Health");
                if (hotkeyBar != null)
                {
                    hotkeyBar.transform.parent.gameObject.SetActive(false);
                    healthBar.SetActive(false);
                }
            }
            else
            {
                Destroy(cursorCanvas);
                freeRotation.m_YAxis.m_InputAxisName = "Mouse Y";
                freeRotation.m_XAxis.m_InputAxisName = "Mouse X";
                //makes UI visible again
                if (hotkeyBar != null)
                {
                    hotkeyBar.transform.parent.gameObject.SetActive(true);
                    AttackScript.pauseDelayTimer = 0.0f;
                }
                if(healthBar != null)
                {
                    healthBar.SetActive(true);
                }
            }
        }
    }
    //spawns in a variety of generic gameObjects that every scene needs
    private void SpawnObjects()
    {
        if (GameObject.FindWithTag("Event_System") == null) //adds eventListener if one not already present
        {
            Instantiate(eventListener, transform.position, transform.rotation);
        }
        if (GameObject.FindWithTag("Hotkey_Bar") == null) //adds HotkeyBarUI if one not already present
        {
            Instantiate(hotkeyUIPrefab, transform.position, transform.rotation);
        }

    }

    public void Respawn()
    {
        firstPlayer = false;
        if (currentCheckpoint != null)
        {
            currentCheckpoint.Respawn();
        }
    }

    public void ClearEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) //don't carry over into the menu
    {
        if (scene.name == "Main_Menu")
        {
            Destroy(gameObject);
        }
    }
}
