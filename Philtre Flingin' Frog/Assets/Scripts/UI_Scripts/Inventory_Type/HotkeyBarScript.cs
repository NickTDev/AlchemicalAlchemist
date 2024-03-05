using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyBarScript : InventoryScript
{
    [SerializeField] float displayScale;
    GameObject itemHandler;
    int currentSelection = 0; //
    [SerializeField] GameObject highlightBorder;
    [SerializeField] GameObject[] itemArray;
    [SerializeField] GameObject emptyBottlePrefab;
    GameObject emptyBottle;
    GameObject infiniteSlot;
    [SerializeField] bool wrapSelection; //whether scrolling past either end of the hotkey bar will wrap to the other side
    int frame = 0; //because of initalizing order stuff, I can't activate the glow on empty bottle till frame 2
    //peepees
    void Update()
    {
        FindPlayer(true); //why does this not work when event triggered? IDK
        for (int i = 1; i < slotList.Count; i++) //ensures the correct potions are displayed in the hotkey bar slots
        {
            RecipeManager.potionType correctType = playerInventory.GetPotionTypeInSlot(i - blankOffsetSlots);
            RecipeManager.secondaryElement correctSecondary = playerInventory.GetPotionSecondaryInSlot(i - blankOffsetSlots);
            GameObject slotItem = itemArray[i];

            if (slotItem != null) //switches currently displayed slot item for the one in this space in the inventory menu
            {
                
                if (slotItem.GetComponent<InventoryItemPotion>().GetPotionType() != correctType || slotItem.GetComponent<InventoryItemPotion>().GetSecondaryType() != correctSecondary)
                {
                    Destroy(slotItem);
                    SpawnSlotItem(slotList[i], i - blankOffsetSlots, 0); //it checks for > 0 itself
                }
                
            }
            else //if there's no item currently in the slot but there needs to be, spawns one
            {
                SpawnSlotItem(slotList[i], i - blankOffsetSlots, 0); //it checks for > 0 itself
            }
        }
        if(!GameManager.GetPaused())
        {
            CheckForHotkey();
        }
        if(frame == 1)
        {
            SetHighlightSlot(0);
        }
        frame++;
    }


    public override void InitTypeVariables()//sets up several variables based on the inventory type
    {
        invType = InventoryType.POTION;
        X_size = (int)playerInventory.GetPotionInventoryDimensions().x + blankOffsetSlots;// (int)RecipeManager.potionType.NUM_POTIONS;
        Y_size = 1;
        itemHandler = GameObject.Find(hierarchyParentName);
    }

    public override void ExtraStartStuff() //spawns empty potion
    {
        SpawnSlotItem(slotList[0], -1, -1);
    }

    public override void SpawnSlotItem(GameObject slot, int x, int y)//creates the manipulatable inventory item object in the slot
    {
        int pos = y * X_size + x; //if is empty bottle, pos will be negative
        if (x == -1 || playerInventory.GetNumPotionInSlot(pos) > 0) //throws infinite index out of bounds errors without this check
        {
            GameObject item = null;
            RecipeManager.potionType type = RecipeManager.potionType.NUM_POTIONS;
            RecipeManager.secondaryElement secondary = RecipeManager.secondaryElement.NUM_SECONDARIES;
            if (x == -1) // x = -1 means empty bottle
            {
                emptyBottle = Instantiate(emptyBottlePrefab, slot.transform.position, slot.transform.rotation) as GameObject;
                item = emptyBottle;
            }
            else //otherwise, figure it out
            {
                type = playerInventory.GetPotionTypeInSlot(pos);
                secondary = playerInventory.GetPotionSecondaryInSlot(pos);
                item = Instantiate(recipeManager.potionPrefabs[(int)type], slot.transform.position, slot.transform.rotation) as GameObject;
            }
            item.transform.SetParent(GameObject.Find(hierarchyParentName).transform, true);
            item.GetComponent<InventoryItemPotion>().SetSecondaryType(secondary);
            item.GetComponent<InventoryItemPotion>().SetScale();
            item.transform.localScale = new Vector3(displayScale * item.transform.localScale.x, displayScale * item.transform.localScale.y, 1);
            itemArray[slot.transform.GetSiblingIndex()] = item; //adds the new item to the array
        }
    }

    protected override GameObject SpawnInventorySlot(int x, int y) //spawns an individual inventory slot (override to fix rotation & size)
    {
        Vector3 slotPos = new Vector3(slotXSpacing * x * displayScale, slotYSpacing * y * -1, 0);
        GameObject slot = Instantiate(slotPrefab, slotPos, transform.rotation) as GameObject;
        slot.transform.SetParent(gameObject.transform, false);
        slot.GetComponent<InventorySlotScript>().Initialize(y * 5 + x);
        slotList.Add(slot);
        slot.transform.localScale = new Vector3(displayScale, displayScale, 1);
        slot.transform.rotation = transform.rotation;
        return slot;
    }

    //sets the selected inventory potion
    void CheckForHotkey()
    {
        if (Input.inputString != "")
        {
            int number;
            bool is_a_number = int.TryParse(Input.inputString, out number);
            if (is_a_number && number > 0 && number <= X_size)
            {
                SetHighlightSlot(number-1);
            }
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            int selectedPos = currentSelection + 1;
            if (selectedPos >= slotList.Count)
            {
                if(wrapSelection)
                {
                    selectedPos = 0;
                }
                else
                {
                    selectedPos = slotList.Count - 1;
                }
            }
            SetHighlightSlot(selectedPos);
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            int selectedPos = currentSelection - 1;
            if (selectedPos < 0)
            {
                if (wrapSelection)
                {
                    selectedPos = slotList.Count - 1;
                }
                else
                {
                    selectedPos = 0;
                }
            }
            SetHighlightSlot(selectedPos);
        }
    }
    void SetHighlightSlot(int slot)
    {
        //first, deactivates the glow on the potion
        if (itemArray[currentSelection] != null)
        {
            itemArray[currentSelection].GetComponent<ScrollOverInteraction>().HotkeyDeselection();
        }
        //then, moves the border on the slot
        currentSelection = slot;
        highlightBorder.transform.position = slotList[slot].transform.position;
        //finally, activates the glow on the next potion
        if (itemArray[slot] != null)
        {
            itemArray[slot].GetComponent<ScrollOverInteraction>().HotkeySelection();
        }
    }

    /*
    private GameObject GetCurrentPotionItem() //returns the potion item in the current slot
    {
        if (currentSelection == 0 || playerInventory.GetNumPotionInSlot(currentSelection - blankOffsetSlots) == 0)//returns empty bottle if selected slot 0 OR none of selected potion available.
        {
            return emptyBottle;
        }
        //must subtract selection by 1 because slot 0 corresponds with [0] in the player's inventory array
        else if (playerInventory.GetNumPotionInSlot(currentSelection - blankOffsetSlots) > 0)
        {
            RecipeManager.potionType type = playerInventory.GetPotionTypeInSlot(currentSelection - blankOffsetSlots);
            RecipeManager.secondaryElement secondary = playerInventory.GetPotionSecondaryInSlot(currentSelection - blankOffsetSlots);
            return item;
        }
    }
    */

    public GameObject GetSelectedPotion() //creates & returns a copy of the currently selected potion
    {
        if (currentSelection == 0 || playerInventory.GetNumPotionInSlot(currentSelection - blankOffsetSlots) == 0)//throws empty bottle if selected slot 0 OR none of selected potion available.
        {
            return emptyBottle;
        }
        //must subtract selection by 1 because slot 0 corresponds with [0] in the player's inventory array
        else if (playerInventory.GetNumPotionInSlot(currentSelection - blankOffsetSlots) > 0)
        {
            RecipeManager.potionType type = playerInventory.GetPotionTypeInSlot(currentSelection - blankOffsetSlots);
            RecipeManager.secondaryElement secondary = playerInventory.GetPotionSecondaryInSlot(currentSelection - blankOffsetSlots);
            GameObject item = Instantiate(recipeManager.potionPrefabs[(int)type], transform.position, transform.rotation) as GameObject;
            item.GetComponent<InventoryItemPotion>().SetSecondaryType(secondary);
            Destroy(item);//destroys after current loop, otherwise it will just hang out
            return item;
        }
        return null;
    }
}
