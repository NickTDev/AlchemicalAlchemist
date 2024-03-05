using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemIngredient : InventoryItem
{
    [SerializeField] RecipeManager.primaryElement prmElm;
    [SerializeField] RecipeManager.secondaryElement scdElm;

    protected override void InitElementSprite()
    {
        RecipeManager recipeManager = GameObject.FindWithTag("Player").GetComponent<RecipeManager>();
        Image displayImage = elementDisplaySprite.GetComponent<Image>(); //gets the sprite's image
        displayImage.color = RecipeManager.colorDictionary[prmElm]; //set the color
        displayImage.sprite = recipeManager.secondarySpritePrefabs[(int)scdElm]; //sets the sprite
        //sets the sprite's text to display the element 'name'
        TMP_Text name_text = elementDisplaySprite.transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        name_text.text = recipeManager.primaryElementNames[(int)prmElm];
    }
    protected override void InitItemPosition()
    {
        itemInventoryX = (int)prmElm;
        itemInventoryY = (int)scdElm;
    }

    protected override void Select() //creates a copy of this item that the player can manipulate
    {
        GameObject moveableItem = Instantiate(gameObject, transform.position, transform.rotation) as GameObject;
        moveableItem.transform.SetParent(GameObject.Find(hierarchyParentName).transform, false);
        moveableItem.transform.SetAsLastSibling(); //to ensure it moves in front of other items
        InventoryItemIngredient itemScript = moveableItem.GetComponent<InventoryItemIngredient>();
        itemScript.SetState(InventoryItem.itemState.SELECTED);
        itemScript.Initialize();
        itemScript.ScaleUp();
        //Moves a portion of the inventory (based on secondary buttons held at the time of click) into the manipulatable item
        int transferAmount = 1;
        if (Input.GetAxisRaw("Select_Big") != 0)
        {
            transferAmount = GetInventoryAmount();
        }
        else if (Input.GetAxisRaw("Select_Med") != 0)
        {
            transferAmount = GetInventoryAmount()/2;
            if(transferAmount == 0)
            {
                transferAmount = 1;
            }
        }
        moveableItem.GetComponent<InventoryItem>().AddAmount(transferAmount);
        ModifyInventory(transferAmount * -1);
        //makes the scale not wonk out
        moveableItem.GetComponent<ScrollOverInteraction>().InitializeMoveableScale(GetComponent<ScrollOverInteraction>());
    }
    protected override void ClickedWhileSelected()
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("Recipe_Ingredient_Slot");
        foreach (GameObject slot in slots)
        {
            RecipeIngredientSlot slotScript = slot.GetComponent<RecipeIngredientSlot>();
            //if the slots A) overlaps the ingredient, B) has the correct primary element, an C) isn't already full with a different ingredient, adds to slot
            if (ItemInSlot(slot) && slotScript.getElement() == prmElm && (slotScript.getIngredient() == null || slotScript.getIngredient().GetComponent<InventoryItemIngredient>().GetName() == itemName))
            {
                SetState(itemState.IN_RECIPE);
                slotScript.InsertIngredient(this.gameObject);
                break;
            } //RectOverlaps(transform.GetChild(0).GetComponent<RectTransform>(), slot.GetComponent<RectTransform>(), transform.localScale, slot.transform.localScale)
        }
    }
    public override void Deselect() //destroys and returns to inventory
    {
        FindHomeObject().GetComponent<InventoryItemIngredient>().ModifyInventory(amount);//returns the SELECTED item's amount to the inventory & updates text
        if (currentState == itemState.IN_RECIPE)//clears it from the recipe
        {
            GameObject[] slots = GameObject.FindGameObjectsWithTag("Recipe_Ingredient_Slot");
            foreach (GameObject slot in slots)
            {
                if (slot.GetComponent<RecipeIngredientSlot>().getIngredient() == this.gameObject)
                {
                    slot.GetComponent<RecipeIngredientSlot>().DeselectIngredient(false);
                }
            }
        }
        Destroy(gameObject);
    }

    protected override GameObject FindHomeObject()//use when SELECTED: finds the copy of this item in the inventory itself
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Inventory_Item");
        foreach (GameObject item in items)
        {
            if (item.GetComponent<InventoryItem>().GetName() == itemName)
            {
                return item;
            }
        }
        return null;
    }

    protected override int GetInventoryAmount() //returns the amount of this item in the inventory
    {
        return playerInventory.GetIngredient(itemInventoryX, itemInventoryY);
    }
    protected override void ModifyInventory(int amnt)//changes the amount of this item in the players inventory
    {
        playerInventory.AddIngredient(itemInventoryX, itemInventoryY, amnt);
    }

    public RecipeManager.primaryElement GetPrimaryElement()
    {
        return prmElm;
    }
    public RecipeManager.secondaryElement GetSecondaryElement()
    {
        return scdElm;
    }
}
