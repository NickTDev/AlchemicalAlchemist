using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//No sounds or art needed here; purely a set-up class

public class InventoryScript : MonoBehaviour
{
    public enum InventoryType
    {
        INGREDIENT,
        POTION
    }
    protected InventoryType invType;
    [SerializeField] protected GameObject slotPrefab;
    protected int slotXSpacing;
    protected int slotYSpacing;
    protected List<GameObject> slotList = new List<GameObject>(); //list of all the slot prefabs in this inventory
    [SerializeField] protected string hierarchyParentName;
    //x & y dimensions of the inventory grid
    protected int X_size;
    protected int Y_size;
    protected RecipeManager recipeManager;
    protected PlayerInventory playerInventory;
    [SerializeField] protected int blankOffsetSlots; //so that the hotkeybar has an free space for the empty potion

    void Start()
    {
        FindPlayer(true);
        slotXSpacing = (int)slotPrefab.GetComponent<RectTransform>().rect.width;
        slotYSpacing = (int)slotPrefab.GetComponent<RectTransform>().rect.height;
        InitTypeVariables();
        GenerateInventorySlots();
        ExtraStartStuff();
    }

    protected void FindPlayer(bool yes)
    {
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        recipeManager = GameObject.FindWithTag("Player").GetComponent<RecipeManager>();
    }


    //generates a grid of inventory slots
    void GenerateInventorySlots()
    {
        for (int y = 0; y < Y_size; y++)
        {
            for (int x = 0; x < X_size; x++)
            {
                GameObject slot = SpawnInventorySlot(x, y);
                if(y * X_size + x >= blankOffsetSlots) //leave a space for empty bottles
                {
                    SpawnSlotItem(slot, x - blankOffsetSlots, y);
                }
            }
        }
    }

    protected virtual GameObject SpawnInventorySlot(int x, int y) //spawns an individual inventory slot
    {
        Vector3 slotPos = new Vector3(slotXSpacing * x, slotYSpacing * y * -1, 0);
        GameObject slot = Instantiate(slotPrefab, slotPos, transform.rotation) as GameObject;
        slot.transform.SetParent(gameObject.transform, false);
        slot.GetComponent<InventorySlotScript>().Initialize(y * 5 + x);
        slotList.Add(slot);
        return slot;
    }

    public GameObject GetSlotObjectAtPosition(int x, int y)
    {
        return slotList[y * X_size + x];
    }
    //virtuals
    public virtual void InitTypeVariables() { }//sets up several variables based on the inventory type
    public virtual void ExtraStartStuff() { }//whatever i need
    public virtual void SpawnSlotItem(GameObject slot, int x, int y) { }//creates the manipulatable inventory item object in the slot
}
