using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public struct potionSlot
    {
        public RecipeManager.potionType type;
        public RecipeManager.secondaryElement secondary;
        public int amount;
    }

    public static bool inventoryUIOpen = false; //whether the inventory UI is currently open
    [SerializeField] GameObject inventoryUIPrefab; //the UI used for the inventory menu
    GameObject inventoryUI; //the UI object currnetly in use

    [SerializeField] int[] startingIngredients;
    int[,] ingredientArray = new int[(int)RecipeManager.primaryElement.NUM_PRIMARIES, (int)RecipeManager.secondaryElement.NUM_SECONDARIES];

    //timer to prevent instant close/reopen
    float openTimer = 0;
    float openGracePeriod = 0.5f;

    //how many slots in the potion inventory
    [SerializeField] int potionXDimension = 5;
    [SerializeField] int potionYDimension = 4;
    potionSlot[] inventoryPotionArray = new potionSlot[5 * 4]; //the potions currently in your inventory

    //any potions you start with in inventory
    [SerializeField] RecipeManager.potionType[] startingPotionTypes;
    [SerializeField] RecipeManager.secondaryElement[] startingPotionSecondaries;
    [SerializeField] int[] startingPotionAmounts;
    [Header("Audio")]
    [SerializeField] FMODUnity.EventReference brewingSound;
    [SerializeField] FMODUnity.EventReference brewingMenuCloseSFX;
    private FMOD.Studio.EventInstance alchemyAudioInstance;
    public bool initialized = false;

    [SerializeField] bool[] collectedTeleportIngredients; //each slot corresponds to the ingredient with that value

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        if(!initialized)
        {
            //initialises all ingredients
            for (int i = 0; i < ingredientArray.GetLength(0); i++)
            {
                for (int j = 0; j < ingredientArray.GetLength(1); j++)
                {
                    int num = 0;
                    int startingPos = j * ingredientArray.GetLength(0) + i;
                    if (startingPos < startingIngredients.Length)
                    {
                        num = startingIngredients[startingPos];
                    }
                    SetIngredient(i, j, num);
                }
            }
            //initialises blank potion slots
            System.Array.Resize(ref inventoryPotionArray, potionXDimension * potionYDimension);
            for (int i = 0; i < inventoryPotionArray.Length; i++)
            {
                SetPotionSlot(i, RecipeManager.potionType.NUM_POTIONS, RecipeManager.secondaryElement.NUM_SECONDARIES, 0);
            }
            //adds any starting potions
            for (int i = 0; i < startingPotionAmounts.Length; i++)
            {
                SetPotionSlot(i, startingPotionTypes[i], startingPotionSecondaries[i], startingPotionAmounts[i]);
            }

            initialized = true;
        }

    }

    void Update()
    {
        HandleUI();
    }
    //////////////////////////////UI
    void HandleUI()
    {
        openTimer -= 1.0f * Time.deltaTime;
        if (inventoryUIOpen && Input.GetAxisRaw("Inventory") != 0 && openTimer < 0.0f)
        {
            CloseUI();
        }
        else if(!inventoryUIOpen && Input.GetAxisRaw("Inventory") > 0 && openTimer < 0.0f && !GameManager.GetPaused()) //won't open inventory if paused
        {
            OpenUI();
        }
    }
    public void OpenUI()
    {
        alchemyAudioInstance = AudioEngineManager.CreateSound(brewingSound,1f);
        alchemyAudioInstance.start();
        inventoryUI = Instantiate(inventoryUIPrefab, transform.position, transform.rotation) as GameObject;
        openTimer = openGracePeriod;
        inventoryUIOpen = true;
       // Cursor.lockState = CursorLockMode.None;
       // Cursor.visible = true;
        GameManager.SetPaused(true);
    }
    public void CloseUI()
    {
        AudioEngineManager.PlaySound(brewingMenuCloseSFX, .5f);
        alchemyAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        DeselectItems(); //so that they return to inventory
        Destroy(inventoryUI);
        openTimer = openGracePeriod;
        inventoryUIOpen = false;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        GameManager.SetPaused(false);
    }

    void DeselectItems()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Inventory_Item");
        foreach (GameObject item in items)
        {
            InventoryItem itemScript = item.GetComponent<InventoryItem>();
            if (itemScript.GetCurrentState() != InventoryItem.itemState.IN_SLOT)
            {
                itemScript.Deselect();
            }
        }
    }


    //////////////////////////////INGREDIENTS
    public void SetIngredient(int x, int y, int amount)
    {
        ingredientArray[x, y] = amount;
    }
    public void AddIngredient(int x, int y, int amount)
    {
        ingredientArray[x, y] += amount;
    }
    public int GetIngredient(int x, int y)
    {
        return ingredientArray[x, y];
    }

    //////////////////////////////POTION SLOTS
    public void SetPotionSlot(int pos, RecipeManager.potionType newType, RecipeManager.secondaryElement newSecondary, int num) //sets all variables in the slot
    {
        inventoryPotionArray[pos].type = newType;
        inventoryPotionArray[pos].secondary = newSecondary;
        inventoryPotionArray[pos].amount = num;
    }

    public void SwapPotionSlots(int pos1, int pos2) //swaps the contents of 2 slots
    {
        RecipeManager.potionType type1 = GetPotionTypeInSlot(pos1);
        RecipeManager.secondaryElement secondary1 = GetPotionSecondaryInSlot(pos1);
        int num1 = GetNumPotionInSlot(pos1);
        SetPotionSlot(pos1, GetPotionTypeInSlot(pos2), GetPotionSecondaryInSlot(pos2), GetNumPotionInSlot(pos2));
        SetPotionSlot(pos2, type1, secondary1, num1);
    }

    //getters
    public RecipeManager.potionType GetPotionTypeInSlot(int slot)
    {
        return inventoryPotionArray[slot].type;
    }
    public RecipeManager.secondaryElement GetPotionSecondaryInSlot(int slot)
    {
        return inventoryPotionArray[slot].secondary;
    }
    public int GetNumPotionInSlot(int slot)
    {
        return inventoryPotionArray[slot].amount;
    }

    public int GetSlotWithPotion(RecipeManager.potionType type, RecipeManager.secondaryElement secondary)//returns the slot containing the potion of the specified type
    {
        for(int i = 0; i < inventoryPotionArray.Length; i++)
        {
            if (GetPotionTypeInSlot(i) == type && GetPotionSecondaryInSlot(i) == secondary)
            {
                return i;
            }
        }
        return -1; //-1 if none currently in inventory
    }
    public int GetNumPotion(RecipeManager.potionType type, RecipeManager.secondaryElement secondary)//returns the number of the potion of the specified type in inventory
    {
        for (int i = 0; i < inventoryPotionArray.Length; i++)
        {
            if (GetPotionTypeInSlot(i) == type && GetPotionSecondaryInSlot(i) == secondary)
            {
                return GetNumPotionInSlot(i);
            }
        }
        return 0; //0 if none currently in inventory
    }
    public Vector2 GetPotionInventoryDimensions()
    {
        return new Vector2(potionXDimension, potionYDimension);
    }

    //setters
    public void SetPotionTypeInSlot(int slot, RecipeManager.potionType newType )
    {
       inventoryPotionArray[slot].type = newType;
    }
    public void SetPotionSecondaryInSlot(int slot, RecipeManager.secondaryElement newSecondary)
    {
        inventoryPotionArray[slot].secondary = newSecondary;
    }
    public void SetNumPotionInSlot(int slot, int num)
    {
        inventoryPotionArray[slot].amount = num;
    }
    private void AddNumPotionInSlot(int slot, int num)
    {
        inventoryPotionArray[slot].amount += num;
    }

    public void AddPotionAmount(RecipeManager.potionType type, RecipeManager.secondaryElement secondary, int amount)//adds the specified amount of the specified potion
    {
        bool alreadyExist = false;
        for (int i = 0; i < inventoryPotionArray.Length; i++) //if this potion is already in the inventory, adds to that slot
        {
            if (GetPotionTypeInSlot(i) == type && GetPotionSecondaryInSlot(i) == secondary)
            {
                bool mustSpawnItem = false;
                if (inventoryUIOpen && GetNumPotionInSlot(i) < 1) //must create new inventory UI item if there are none
                {
                    mustSpawnItem = true;
                }
                AddNumPotionInSlot(i, amount);
                alreadyExist = true;
                if(mustSpawnItem) //the item must be spawned because inventoryScript requires there to be at least 1 item in order to spawn
                {
                    PotionInventoryScript potionInventoryScript = GameObject.Find("Potion_Inventory").GetComponent<PotionInventoryScript>();
                    potionInventoryScript.SpawnSlotItem(potionInventoryScript.GetSlotObjectAtPosition(i % potionXDimension, i / potionXDimension), i % potionXDimension, i / potionXDimension);
                }
                break;
            }
        }
        if(!alreadyExist) //creates new instance of the potion in empty slot otherwise
        {
            for (int i = 0; i < inventoryPotionArray.Length; i++) //finds empty slot
            {
                if (GetNumPotionInSlot(i) < 1)
                {
                    SetPotionSlot(i, type, secondary, amount);
                    if(inventoryUIOpen)//spawns visual representation
                    {
                        PotionInventoryScript potionInventoryScript = GameObject.Find("Potion_Inventory").GetComponent<PotionInventoryScript>();
                        potionInventoryScript.SpawnSlotItem(potionInventoryScript.GetSlotObjectAtPosition(i % potionXDimension, i / potionXDimension), i % potionXDimension, i / potionXDimension);
                    }
                    break;
                }
            }
        }
    }

    //////////////////////////////TELEPORTATION IGNEDIENTS
    public void CollectTeleportIngredient(int ingredientValue)
    {
        collectedTeleportIngredients[ingredientValue] = true;
    }
    public bool HasTeleportIngredient(int ingredientValue)
    {
        return collectedTeleportIngredients[ingredientValue];
    }

    //////////////////////////////GETTERS AND SETTERS
    public void SetIngredientArray(int[,] inputArray)
    {
        ingredientArray = inputArray;
    }
    public int[,] GetIngredientArray()
    {
        return ingredientArray;
    }

    public void SetPotionArray(potionSlot[] inputArray)
    {
        inventoryPotionArray = inputArray;
    }
    public potionSlot[] GetPotionArray()
    {
        return inventoryPotionArray;
    }

    public void SetTeleportArray(bool[] inputArray)
    {
        collectedTeleportIngredients = inputArray;
    }
    public bool[] GetTeleportArray()
    {
        return collectedTeleportIngredients;
    }
}
