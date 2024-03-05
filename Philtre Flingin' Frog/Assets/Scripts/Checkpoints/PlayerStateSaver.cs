using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSaver : MonoBehaviour
{
    int[,] savedIngredientArray = new int[(int)RecipeManager.primaryElement.NUM_PRIMARIES, (int)RecipeManager.secondaryElement.NUM_SECONDARIES];
    PlayerInventory.potionSlot[] savedInventoryPotionArray = new PlayerInventory.potionSlot[5 * 4];
    bool[] potionsUnlocked; //whether the recipe at each slot has been unlocked
    bool[] savedTeleportationIngredientsArray; //whether each artifact is collected

    protected void SavePlayerInventory(GameObject player) //must create a copy of each array because otherwise they get passed by reference and deleted
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        //saves ingredient inventory
        int[,] tempSavedIngredientArray = inventory.GetIngredientArray();
        for (int i = 0; i < (int)RecipeManager.primaryElement.NUM_PRIMARIES; i++)
        {
            for (int j = 0; j < (int)RecipeManager.secondaryElement.NUM_SECONDARIES; j++)
            {
                savedIngredientArray[i, j] = tempSavedIngredientArray[i, j];
            }
        }
        //saves potion inventory
        PlayerInventory.potionSlot[] tempSavedInventoryPotionArray = inventory.GetPotionArray();
        for (int i = 0; i < 20; i++)
        {
            savedInventoryPotionArray[i] = tempSavedInventoryPotionArray[i];
        }
        //saves whether each recipe is unlocked
        GameObject[] potionArray = RecipeManager.GetPotionArray();
        potionsUnlocked = new bool[RecipeManager.GetNumPotionTypes()]; //ensures correct size
        for (int i = 0; i < potionArray.Length; i++)
        {
            potionsUnlocked[i] = potionArray[i].GetComponent<InventoryItemPotion>().GetUnlocked();
        }

        //teleportation ingredients
        bool[] tempTeleportArray = inventory.GetTeleportArray();
        savedTeleportationIngredientsArray = new bool[tempTeleportArray.Length];
        for (int i = 0; i < tempTeleportArray.Length; i++)
        {
            savedTeleportationIngredientsArray[i] = tempTeleportArray[i];
        }
    }

    protected void LoadPlayerInventory(GameObject player)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        inventory.SetIngredientArray(savedIngredientArray);
        inventory.SetPotionArray(savedInventoryPotionArray);
        inventory.SetTeleportArray(savedTeleportationIngredientsArray);
        //loads which recipes are unlocked
        GameObject[] potionArray = RecipeManager.GetPotionArray();
        for (int i = 0; i < potionArray.Length; i++)
        {
            if (potionsUnlocked[i])
            {
                RecipeManager.UnlockRecipe(potionArray[i]);
            }
        }
    }
}