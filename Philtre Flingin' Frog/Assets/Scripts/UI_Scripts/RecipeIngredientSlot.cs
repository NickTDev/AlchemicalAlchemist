using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeIngredientSlot : MonoBehaviour
{
    GameObject parentRecipe;
    GameObject currentIngredient = null;
    RecipeManager.primaryElement element;

    public void Initialize(GameObject parent, RecipeManager.primaryElement elm)
    {
        parentRecipe = parent;
        element = elm;
        GetComponent<Image>().color = RecipeManager.colorDictionary[element];
        transform.SetSiblingIndex(1);
        //displays the element 'name'
        RecipeManager recipeManager = GameObject.FindWithTag("Player").GetComponent<RecipeManager>();
        TMP_Text name_text = transform.GetChild(0).gameObject.GetComponent<TMP_Text>();
        name_text.text = recipeManager.primaryElementNames[(int)element];
    }
    public RecipeManager.primaryElement getElement() //returns this slots primary element type
    {
        return element;
    }
    public GameObject getIngredient()
    {
        return currentIngredient;
    }
    public int getIngredientAmount()
    {
        return currentIngredient.GetComponent<InventoryItem>().GetAmount();
    }

    public void DeactivateSlot()
    {
        DeselectIngredient(true);
        Destroy(gameObject);
    }

    public void DeselectIngredient(bool callDeseletOnIngredient) //deslects the current ingredient, returning it to the inventory
    {
        if(callDeseletOnIngredient && currentIngredient != null)
        {
            currentIngredient.GetComponent<InventoryItem>().Deselect();
        }
        currentIngredient = null;
    }

    public void InsertIngredient(GameObject newIngredient)
    {
        if (currentIngredient == null)
        {
            currentIngredient = newIngredient;
            currentIngredient.transform.position = transform.position;
        }
        else if(currentIngredient.GetComponent<InventoryItem>().GetName() == newIngredient.GetComponent<InventoryItem>().GetName())//if ingredient already inserted, adds new to old
        {
            currentIngredient.GetComponent<InventoryItem>().AddAmount(newIngredient.GetComponent<InventoryItem>().GetAmount());
            Destroy(newIngredient);
        }
        parentRecipe.GetComponent<Recipe>().IngredientAdded();
    }
    public void RecipeCompleted(int amount) //uses up ingredients
    {
        currentIngredient.GetComponent<InventoryItem>().AddAmount(amount * -1);
        if(currentIngredient.GetComponent<InventoryItem>().GetAmount() <= 0)
        {
            Destroy(currentIngredient);
            currentIngredient = null;
        }
    }

    public RecipeManager.secondaryElement GetSecondaryType()
    {
        return currentIngredient.GetComponent<InventoryItemIngredient>().GetSecondaryElement();
    }
}