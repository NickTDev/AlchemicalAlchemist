using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Recipe : MonoBehaviour
{
    [SerializeField] GameObject slotPrefab;
    GameObject potionItem; //the actual potion this makes
    GameObject potionVisual = null; //the object displaying the potion sprite
    GameObject ingredientParent = null; //the object telling the ingredients where to display
    RecipeManager.potionType type;
    RecipeManager.secondaryElement secondaryType = RecipeManager.secondaryElement.NUM_SECONDARIES;
    RecipeManager.primaryElement[] elements; //gets as input from the potion object
    GameObject[] ingredients;
    List<GameObject> slotList = new List<GameObject>();
    string recipeName;
    string recipeDescription;
    [SerializeField] FMODUnity.EventReference brewSound;
    Image dropShadow; //the image used for the dorp shadow when you mouse over
    [SerializeField] float hoverScale; //how large the recipe grows when you mouse over
    float baseScale; //the scale to return to
    bool isUpgrade = false; //wether this recipe is appearing on the upgrade menu
    [SerializeField] Color unlockableColor;
    [SerializeField] Color lockedColor;
    Color unlockedColor; //basic color
    bool isUnlocked = true; //whether this potion is available
    bool unlockable = false; //whether this potion is avialable to be unlocked in the upgrade menu
    [SerializeField] Sprite[] emptyBottles; //the sprites used for unfilled potions
    [SerializeField] float unfilledAlpha; //how transparent potions should be when slots arent filled
    Image centerHighlightSprite; //the sprite highlighting the bottle in the center
    Image centerBottleSprite; //the empty bottle sprite in the center
    Image centerPotionSprite; //the sprite representing the actual potion in the center
    bool slotsFilled = false; //whether all ingredient slots are filled
    bool wasFilled = false; //whether all ingredient slots WERE filled last frame
    int ingredientAvailable = 1000; //the fewest ingredients avaialable in any slot
    bool isActive = false; //whether this is the selected recipe
    [SerializeField] float trigPosFactor; //how much the curve affects ingredient slot placement
    [SerializeField] int yeildPerIngredient = 1; //how many potoins you get for each ingredient in slots
    UpgradeMenuScript upgradeMenu;
    RecipeSpawnerScript recipeSpawner;


    public void Initialize(GameObject inputPotion, bool upgrade)
    {
        baseScale = transform.localScale.x;
        hoverScale *= baseScale;
        recipeSpawner = transform.parent.gameObject.GetComponent<RecipeSpawnerScript>();
        //gets info from the potion item
        potionItem = inputPotion;
        InventoryItemPotion potionScript = potionItem.GetComponent<InventoryItemPotion>();
        type = potionScript.GetPotionType();
        elements = potionScript.GetElements();
        //sets up the various children that display information
        foreach (Transform child in transform)
        {
            if (child.name == "Display_Image")//sets the recipe to use the same sprite as the potion, so it only HAS to be updated in one place
            {
                potionVisual = child.gameObject;
                potionVisual.GetComponent<Image>().sprite = potionItem.transform.GetChild(0).GetComponent<Image>().sprite;
            }
            else if (child.name == "Name_Text")
            {
                child.gameObject.GetComponent<TMP_Text>().text = potionScript.GetName() + " (" + potionScript.GetCategory() + " " + potionScript.GetTier() + ")";
            }
            else if (child.name == "Specification_Text")
            {
                child.gameObject.GetComponent<TMP_Text>().text = potionScript.GetSpecs();
            }
            else if (child.name == "Flavor_Text")
            {
                child.gameObject.GetComponent<TMP_Text>().text = potionScript.GetFlavor();
            }
            else if (child.name == "Ingredient_Parent")
            {
                ingredientParent = child.gameObject;
            }
        }
        //spawns the ingredient list
        for (int i = 0; i < elements.Length; i++)
        {
            //position
            float slotXPos = ingredientParent.transform.localScale.x * slotPrefab.GetComponent<RectTransform>().rect.width * 1.2f * i;
            Vector3 slotPos = new Vector3(slotXPos, 0, 0);
            GameObject slot = Instantiate(slotPrefab, slotPos, transform.rotation) as GameObject;
            //other stuff
            slot.transform.SetParent(ingredientParent.transform, false);
            slot.GetComponent<RecipeIngredientSlot>().Initialize(this.gameObject, elements[i]);
            slot.GetComponent<RecipeIngredientSlot>().enabled = false;
            slot.transform.localScale = new Vector3(ingredientParent.transform.localScale.x, ingredientParent.transform.localScale.y, ingredientParent.transform.localScale.z);
            slot.tag = "Untagged"; //the default tag will cause it to be deleted on selecting a recipe
        }
        dropShadow = GetComponent<Image>();
        //deals with upgrade
        if (upgrade)
        {
            isUpgrade = true;
            unlockedColor = transform.GetChild(0).GetComponent<Image>().color;
            isUnlocked = potionScript.GetUnlocked();
            if (!isUnlocked)
            {
                transform.GetChild(0).GetComponent<Image>().color = lockedColor;
            }
        }
        //Figures out which sprites are which on the brew button
        else
        {
            GameObject centerButton = GameObject.Find(RecipeManager.originObjectName);
            centerHighlightSprite = centerButton.GetComponent<Image>(); //first sets the background glow to the correct bottle shape
            centerBottleSprite = centerButton.transform.GetChild(0).GetComponent<Image>(); //then sets the empty bottle sprite
            centerPotionSprite = centerButton.transform.GetChild(1).GetComponent<Image>(); //then sets the actual bottle to the correct potion
        }
    }

    public void SetUpgradeMenu(UpgradeMenuScript script)
    {
        upgradeMenu = script;
    }

    void Update()
    {
        if (isActive)
        {
            CheckIfFilled();
            if (slotsFilled && !wasFilled) //if it needs to activate highlight, does so
            {
                VisuallyActivate();
            }
            else if (!slotsFilled && wasFilled) //if it needs to deactivate highlight, does so
            {
                VisuallyDeactivate();
            }
            wasFilled = slotsFilled;
        }
    }
    void VisuallyActivate()
    {
        centerHighlightSprite.enabled = true;
        centerBottleSprite.enabled = false;
        centerPotionSprite.color = new Color(1f, 1f, 1f, 1f);
    }
    void VisuallyDeactivate()
    {
        centerHighlightSprite.enabled = false;
        centerBottleSprite.enabled = true;
        centerPotionSprite.color = new Color(1f, 1f, 1f, unfilledAlpha);
    }
    void CheckIfFilled()
    {
        slotsFilled = true;
        ingredientAvailable = 1000; //the least ingredient available in any slot
        if (potionItem.GetComponent<InventoryItemPotion>().GetUnlimited()) //default empty bottle shouldn't light up
        {
            slotsFilled = false;
        }
        else
        {
            for (int i = 0; i < slotList.Count; i++) //checks if all slots are correctly filled
            {
                RecipeIngredientSlot slot = slotList[i].GetComponent<RecipeIngredientSlot>();
                if (slot.getIngredient() == null) //fails if any slot is empty
                {
                    slotsFilled = false;
                    break;
                }
                else //otherwise keeps track of how many ingredients available
                {
                    int amnt = slot.getIngredientAmount();
                    if (amnt < ingredientAvailable)
                    {
                        ingredientAvailable = amnt;
                    }
                }
            }
        }
    }
    public void Click()
    {
        if (!isUpgrade) //if in brewing menu, sets as active recipe
        {
            ActivateRecipe();
        }
        else if (isUpgrade && unlockable) //if in upgrade menu, unlocks recipe
        {
            UnlockRecipe();
        }
    }
    void UnlockRecipe()
    {
        RecipeManager.UnlockRecipe(type);
        transform.GetChild(0).GetComponent<Image>().color = unlockedColor;
        unlockable = false;
        isUnlocked = true;
        //sets the next recipe as unlockable NO IT DOESN'T ONLY ONE AT A TIME
        /*
        if (transform.GetSiblingIndex() + 1 < transform.parent.childCount) //otherwise would throw an out of bounds error if this is the last recipe
        {
            Recipe nextRecipe = transform.parent.GetChild(transform.GetSiblingIndex() + 1).GetComponent<Recipe>();
            if (!nextRecipe.GetUnlocked())
            {
                nextRecipe.SetUnlockable(true);
            }
        }
        */
        upgradeMenu.Unlocked();
    }
    void ActivateRecipe()
    {
        isActive = true;
        GameObject activatedRecipe = recipeSpawner.GetRecipe();
        if (activatedRecipe != null)
        {
            activatedRecipe.GetComponent<Recipe>().DeactivateRecipe();
        }
        //sets the sprite in the center to show the current recipe
        InventoryItemPotion potionScript = potionItem.GetComponent<InventoryItemPotion>();
        centerHighlightSprite.sprite = potionScript.GetBottleGlow();//first sets the background glow to the correct bottle shape
        centerBottleSprite.sprite = emptyBottles[potionScript.GetTier() - 1]; //then sets the actual bottle to the correct potion
        centerPotionSprite.sprite = potionScript.GetItemSprite();
        //spawns the ingredient slots        RecipeManager.slotXOffsets[elements.Length-1] + RecipeManager.slotXSpacing * i,
        float slotXSize = slotPrefab.GetComponent<RectTransform>().rect.width;
        float slotYSize = slotPrefab.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < elements.Length; i++)
        {
            float relativePosition = i - (elements.Length - 1) / 2.0f;
            float trigOffset = relativePosition * relativePosition * relativePosition * relativePosition * trigPosFactor;
            if (relativePosition > 0)
            {
                trigOffset *= -1;
            }
            float slotXPos = (RecipeManager.slotXSpacing + slotXSize) * relativePosition + trigOffset;
            float slotYPos = RecipeManager.slotDefaultY - (Mathf.Abs(relativePosition) * slotYSize / 1.5f) - Mathf.Abs(trigOffset) / 1.5f;
            //I hate all of that math I just did. Should really have just used actual trig
            Vector3 slotPos = new Vector3(slotXPos, slotYPos, 0);
            GameObject slot = Instantiate(slotPrefab, slotPos, transform.rotation) as GameObject;
            slot.transform.SetParent(GameObject.FindGameObjectWithTag("Inventory_UI").transform, false);
            slot.GetComponent<RecipeIngredientSlot>().Initialize(this.gameObject, elements[i]);
            slotList.Add(slot);
        }
        recipeSpawner.SetRecipe(gameObject);
        //make sure to reset everything
        slotsFilled = false;
        wasFilled = false;
        VisuallyDeactivate();
    }
    void DeactivateRecipe()
    {
        isActive = false;
        GameObject[] slots = GameObject.FindGameObjectsWithTag("Recipe_Ingredient_Slot");
        foreach (GameObject slot in slots)
        {
            slot.GetComponent<RecipeIngredientSlot>().DeactivateSlot();
        }
        slotList.Clear();
    }
    public void IngredientAdded() //called when an ingredient is added
    {
    }
    public void CheckForBrew()
    {
        if (slotsFilled) //returns success if no slot is missing
        {
            createPotion(ingredientAvailable * yeildPerIngredient);
        }
    }
    void createPotion(int num) //uses up the ingredients
    {
        AudioEngineManager.CreateSound(brewSound, 1f).start();
        GameObject[] slots = GameObject.FindGameObjectsWithTag("Recipe_Ingredient_Slot");
        secondaryType = slots[0].GetComponent<RecipeIngredientSlot>().GetSecondaryType();
        foreach (GameObject slot in slots)
        {
            RecipeIngredientSlot slotScript = slot.GetComponent<RecipeIngredientSlot>();
            if (slotScript.GetSecondaryType() != secondaryType) //if any slot has different secondary, potion does not get secondary
            {
                secondaryType = RecipeManager.secondaryElement.NUM_SECONDARIES;
            }
            slotScript.RecipeCompleted(num);
        }
        ModifyInventory(num);
    }
    void ModifyInventory(int amnt)//changes the amount of this item in the players inventory
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerInventory>().AddPotionAmount(type, secondaryType, amnt);
    }
    public void MouseOver() //highlights recipe when you mouse over
    {
        if (!isUpgrade || unlockable)
        {
            dropShadow.enabled = true;
            transform.localScale = new Vector3(hoverScale, hoverScale, hoverScale);
            //transform.SetAsLastSibling(); //to ensure it's not hidden behind others, breaks unlocked the next recipe tho
        }
    }
    public void MouseOff() //removes the mouse over highlight
    {
        dropShadow.enabled = false;
        transform.localScale = new Vector3(baseScale, baseScale, baseScale);
    }
    public void SetUnlockable(bool canUnlock)
    {
        unlockable = canUnlock;
        if (canUnlock && !isUnlocked)
        {
            transform.GetChild(0).GetComponent<Image>().color = unlockableColor;
        }
        else if(!canUnlock && !isUnlocked)
        {
            transform.GetChild(0).GetComponent<Image>().color = lockedColor;
        }
    }
    public bool GetUnlocked()
    {
        return isUnlocked;
    }
    public bool GetIsFilled()
    {
        return slotsFilled;
    }
}