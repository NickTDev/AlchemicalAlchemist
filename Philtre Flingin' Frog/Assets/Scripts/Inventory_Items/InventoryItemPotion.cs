using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemPotion : InventoryItem
{
    [SerializeField] string potionCategory = "Potion";
    [SerializeField] int potionTier = 0;
    [SerializeField] RecipeManager.potionType type;
    [SerializeField] Color potionColor; //will be applied to the projectile & the AOE
    RecipeManager.secondaryElement secondaryType = RecipeManager.secondaryElement.NUM_SECONDARIES;
    PotionBehaviour potionBehaviour;
    [SerializeField] RecipeManager.primaryElement[] elements;
    bool unlocked = false; //all potions start off locked by default
    GameObject inventorySlot; //the slot that contains this potion
    [SerializeField] Sprite[] bottleHighlights; //the sprites used to highlight filled bottles


    public void SetSecondaryType(RecipeManager.secondaryElement scnd)
    {
        secondaryType = scnd;
    }

    protected override void InitElementSprite()
    {
        RecipeManager recipeManager = GameObject.FindWithTag("Player").GetComponent<RecipeManager>();
        Image displayImage = elementDisplaySprite.GetComponent<Image>(); //gets the sprite's image
        displayImage.sprite = recipeManager.secondarySpritePrefabs[(int)secondaryType]; //sets the sprite
    }
    protected override void InitItemPosition()
    {
        itemInventoryX = (int)type;
        itemInventoryY = (int)secondaryType;
        destroyOnZero = true;
    }

    protected override void Select() //creates a copy of this item that the player can manipulate
    {
        SetState(itemState.SELECTED);
        transform.SetAsLastSibling(); //to ensure it moves in front of other items
    }

    protected override void ClickedWhileSelected()
    {
        GameObject potionSlots = GameObject.Find("Potion_Inventory");
        foreach (Transform slot in potionSlots.transform)
        {
            InventorySlotScript slotScript = slot.GetComponent<InventorySlotScript>();
            //if the slot A) overlaps the potion, and B) is empty, moves the potion to that slot
            if (slotScript != null && ItemInSlot(slot.gameObject))
            {//RectOverlaps(gameObject.GetComponent<RectTransform>(), slot.GetComponent<RectTransform>())
                SetState(itemState.IN_SLOT);
                //swaps positions
                transform.position = slot.position;
                InventoryItemPotion[] potions = GameObject.FindObjectsOfType<InventoryItemPotion>();
                foreach (InventoryItemPotion potion in potions) //determines if there's another potion already occuping this slot. If so, moves it physically to old slot
                {
                    if(potion != this && ItemInSlot(potion.gameObject)) //don't overlap self
                    { //RectOverlaps(gameObject.GetComponent<RectTransform>(), potion.GetComponent<RectTransform>())
                        potion.transform.position = inventorySlot.transform.position;
                        potion.SetSlot(inventorySlot);
                        break;
                    }
                }
                //swaps the actual inventory amounts
                playerInventory.SwapPotionSlots(inventorySlot.GetComponent<InventorySlotScript>().GetSlotIndex(), slotScript.GetSlotIndex());
                SetSlot(slot.gameObject);
                break;
            }
        }
    }
    public override void Deselect() //returns to slot
    {
        SetState(itemState.IN_SLOT);
        transform.position = inventorySlot.transform.position;
    }
    protected override int GetInventoryAmount() //returns the amount of this item in the inventory
    {
        return playerInventory.GetNumPotion(type, secondaryType);
    }

    public RecipeManager.potionType GetPotionType()
    {
        return type;
    }
    public RecipeManager.secondaryElement GetSecondaryType()
    {
        return secondaryType;
    }

    public Color GetPotionColor()
    {
        return potionColor;
    }
    public string GetCategory()
    {
        return potionCategory;
    }
    public int GetTier()
    {
        return potionTier;
    }

    public RecipeManager.primaryElement[] GetElements()
    {
        return elements;
    }

    public bool GetUnlocked()
    {
        return unlocked;
    }
    public void SetUnlocked(bool unlock)
    {
        unlocked = unlock;
    }

    public void SetSlot(GameObject slot)
    {
        inventorySlot = slot;
    }

    public Sprite GetBottleGlow() //returns the correct glow to outline this potion
    {
        return bottleHighlights[potionTier - 1];
    }

}
