using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    public enum primaryElement
    {
        RED,
        BLUE,
        WHITE,
        BLACK,
        NUM_PRIMARIES
    }
    public enum secondaryElement
    {
        SQUARE,
        CIRCLE,
        TRIANGLE,
        NUM_SECONDARIES
    }
    public enum potionType
    {
        DAMAGE,
        IGNITION,
        EXPLOSION,
        POISON,
        FLAMMABLE,
        LIKE_REALLY_TOXIC,
        GOO,
        BIG_GOO,
        FREEZE,
        HEALTH,
        PURITY,
        INVULNERABLE,
        CONFUSION,
        FEAR,
        MADNESS,
        NUM_POTIONS
    }
    /*public enum potionSecondaryType
    {
        NORMAL,
        CLOUD,
        NUM_SECONDARY_POTION_TYPES
    }*/
    public static Dictionary<primaryElement, Color> colorDictionary = new Dictionary<primaryElement, Color>(); //the colors for each primary element
    public static int[] slotXOffsets = new int[]{ 0, -45 }; //how much the leftmost slot's X will be offset at different numebrs of total slots
    public static int slotDefaultX = 0; //default X position for recipe slots relative to origin
    public static int slotXSpacing = 10; //X distance between recipe slots
    public static int slotDefaultY = 175; //default Y position for recipe slots relative to origin
    public static string originObjectName = "Current_Recipe_Visual";
    public GameObject[] ingredientPrefabs;
    public GameObject[] potionPrefabs; //all potential potions
    [SerializeField] bool unlockAllRecipes; //if true, will automatically unlock all recipes
    [SerializeField] GameObject[] unlockedRecipes; //potions the player knows how to make on startup
    public GameObject[] deliveryMethodPrefabs;
    public Sprite[] secondarySpritePrefabs; //sprites for the secondary elements
    public string[] primaryElementNames;
    public Color[] primaryElementColors;
    static RecipeManager instance;

    void Awake() //fills the dictionaries
    {
        instance = this;
        //adds the correct colors to the dictionary
        if(colorDictionary.Count == 0) //
        {
            for (int i = 0; i < (int)primaryElement.NUM_PRIMARIES; i++)
            {
                colorDictionary.Add((primaryElement)i, primaryElementColors[i]);
            }
        }
        if(unlockAllRecipes)
        {
            foreach (GameObject potion in potionPrefabs)
            {
                UnlockRecipe(potion);
            }
        }
        else
        {
            foreach (GameObject potion in potionPrefabs) //for some reason I have to lock them all manually
            {
                potion.GetComponent<InventoryItemPotion>().SetUnlocked(false);
            }
            foreach (GameObject potion in unlockedRecipes)
            {
                UnlockRecipe(potion);
            }
        }
    }
    public GameObject GetIngredient(primaryElement primary, secondaryElement secondary)
    {
        for (int i = 0; i < ingredientPrefabs.Length; i++)
        {
            InventoryItemIngredient ingredientScript = ingredientPrefabs[i].GetComponent<InventoryItemIngredient>();
            if (ingredientScript.GetPrimaryElement() == primary && ingredientScript.GetSecondaryElement() == secondary)
            {
                return ingredientPrefabs[i];
            }
        }
        return null;
    }

    //unlocks the given recipe
    public static void UnlockRecipe(GameObject recipe)
    {
        string recipeName = recipe.GetComponent<InventoryItem>().GetName();
        foreach (GameObject potion in instance.potionPrefabs)
        {
            if(potion.GetComponent<InventoryItem>().GetName() == recipeName)
            {
                potion.GetComponent<InventoryItemPotion>().SetUnlocked(true);
                break;
            }
        }
    }
    /* Avoid: potion names are subject to change
    public void UnlockRecipe(string recipeName)
    {
        foreach (GameObject potion in potionPrefabs)
        {
            if (potion.GetComponent<InventoryItem>().GetName() == recipeName)
            {
                potion.GetComponent<InventoryItemPotion>().SetUnlocked(true);
                break;
            }
        }
    }
    */
    public static void UnlockRecipe(potionType recipe)
    {
        if(instance != null)
        {
            foreach (GameObject potion in instance.potionPrefabs)
            {
                if (potion.GetComponent<InventoryItemPotion>().GetPotionType() == recipe)
                {
                    potion.GetComponent<InventoryItemPotion>().SetUnlocked(true);
                    break;
                }
            }
        }
    }
    public static int GetNumPotionTypes()
    {
        if (instance != null)
        {
            return instance.potionPrefabs.Length;
        }
        else
        {
            return 0;
        }
    }
    public static GameObject[] GetPotionArray()
    {
        if (instance != null)
        {
            return instance.potionPrefabs;
        }
        else
        {
            return new GameObject[0];
        }
    }
}