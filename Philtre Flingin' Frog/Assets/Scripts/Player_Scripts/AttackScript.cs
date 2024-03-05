using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class AttackScript : MonoBehaviour
{
    public GameObject projectile;
    public Transform orientation;
    public Transform launchPoint;
    static bool leftClick;
    PlayerInventory inventoryScript;
    RecipeManager recipeManager;
    static float timeClicked;
    GameObject bar;
    Slider slider;
    public float maxHeldDown = .5f;
    public float timeHeldDown = 0.0f;
    [SerializeField] GameObject chargeThrowUIPrefab;
    GameObject chargeBar;
    Projectile proj;
    [SerializeField] float cooldown;
    float cooldownTimer;

    [SerializeField] FMODUnity.EventReference ThrowSFX;

    public static float pauseDelayTimer = 0;
    float pauseDelay = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        proj = projectile.GetComponent<Projectile>();
        maxHeldDown = proj.maxHeldDown;
        inventoryScript = gameObject.GetComponent<PlayerInventory>();
        recipeManager = gameObject.GetComponent<RecipeManager>();

        //adds chargeThrowUI if one not already present
        chargeBar = GameObject.FindWithTag("Charge_Throw_UI");
        if (chargeBar == null)
        {
            chargeBar = Instantiate(chargeThrowUIPrefab, transform.position, transform.rotation) as GameObject;
        }
        bar = chargeBar.transform.GetChild(0).gameObject;
        slider = bar.GetComponent<Slider>();
        cooldownTimer = cooldown; //can throw on second frame
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            timeHeldDown = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!GameManager.GetPaused())
        {
            //cooldown timer
            cooldownTimer += Time.deltaTime;
            pauseDelayTimer += Time.deltaTime;
            if (cooldownTimer > cooldown && pauseDelayTimer > pauseDelay)
            {
                //count seconds with time.deltaTime instead, this causes odditys
                if (Input.GetMouseButtonDown(0))
                { 
                    timeHeldDown = 0.0f;
                }
                if (Input.GetMouseButton(0))
                { 
                    timeHeldDown += Time.deltaTime;
                }
                if (timeHeldDown > maxHeldDown)
                {
                    timeHeldDown = maxHeldDown;
                }

                if (Input.GetMouseButtonUp(0) && GameObject.FindWithTag("Inventory_UI") == null)
                { 
                    timeClicked = timeHeldDown;
                    leftClick = true;
                    ThrowPotion();
                }

                if (Input.GetMouseButtonUp(1) && GameObject.FindWithTag("Inventory_UI") == null)
                {
                    leftClick = false;
                    ThrowPotion();
                }
            }
        }
        if(timeHeldDown > 0.1)
        {
            slider.value = timeHeldDown / maxHeldDown;
        }
        else
        {
            slider.value = 0;
        }
    }

    void ThrowPotion()
    {

        GameObject potionType = GameObject.FindWithTag("Hotkey_Bar").GetComponent<HotkeyBarScript>().GetSelectedPotion();
        if (potionType != null) //can only attack if you have any of that potion available
        {
            AudioEngineManager.PlaySound(ThrowSFX, 1f, this.gameObject);
            //gets info about the potion
            InventoryItemPotion potionScript = potionType.GetComponent<InventoryItemPotion>();
            PotionBehaviour behaviour = potionType.GetComponent<PotionBehaviour>(); //the script to apply to the new projectile
            if (!potionScript.GetUnlimited()) //only subtract form inventory if NOT unlimited
            {
                int slotWithPotion = inventoryScript.GetSlotWithPotion(potionScript.GetPotionType(), potionScript.GetSecondaryType());
                inventoryScript.AddPotionAmount(potionScript.GetPotionType(), potionScript.GetSecondaryType(), -1);//subtracts one of this potion type
            }
            //creates & launches the potion
            GameObject item = Instantiate(projectile, launchPoint.position, orientation.rotation);
            //creates a potion inventory item with the correct potion behaviour script + another item with the delivery method & attaches them to the projectile
            GameObject potionChild = Instantiate(potionType, launchPoint.position, orientation.rotation);
            GameObject deliveryChild = Instantiate(recipeManager.deliveryMethodPrefabs[(int)potionScript.GetSecondaryType()], launchPoint.position, orientation.rotation);
            item.GetComponent<Projectile>().SetPotionType(potionChild, deliveryChild);
            //resets the timer
            cooldownTimer = 0;
        }
    }
    public bool getLeftClick()
    {
        return leftClick;
    }

    public float getTimeClicked()
    {
        return timeClicked;
    }
}
