using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RecipeSpawnerScript : MonoBehaviour
{
    [SerializeField] TMP_Text headerText;
    [SerializeField] string recipeHeader;
    [SerializeField] string deliveryHeader;
    [SerializeField] RectTransform heightObject;
    [SerializeField] Transform deliveryMethodParent; //the object that contains the delivery method displays
    [SerializeField] GameObject recipePrefab;
    [SerializeField] int recipeYSpacing; //the space between recipes
    GameObject currentRecipe;
    [SerializeField] float scrollSensitivity = 1;
    float screenScale;
    bool displayRecipes = true; //whether displaying recipes as opposed to delivery methods
    float minScrollDistance; //how far up you can scroll the recipe list
    float maxScrollDistance; //how far down you can scroll the recipe list

    void Start() //spawns the recipes
    {
        RecipeManager recipeManager = GameObject.FindWithTag("Player").GetComponent<RecipeManager>();
        float recipeYSize = recipePrefab.GetComponent<RectTransform>().rect.height * recipePrefab.transform.localScale.y;
        //scollable distance
        minScrollDistance = transform.position.y;
        maxScrollDistance = transform.TransformPoint(transform.position).y; //why does max need transformPoint but min doesn't? No fucking clue
        maxScrollDistance -= (heightObject.rect.height * heightObject.transform.localScale.y) - 30;//don't want the final recipe to go off screen, should display at bottom of window
        //spanws the recipes themselves
        int position = 0;
        foreach (GameObject recipe in recipeManager.potionPrefabs)
        {
            if(recipe.GetComponent<InventoryItemPotion>().GetUnlocked())
            {
                float yPos = recipeYSize * -position + recipeYSpacing * -(position - 1);
                Vector3 num = new Vector3(0, yPos, 0); //((recipeYSpacing + recipeYSize) * (i - (recipeManager.potionPrefabs.Length-1) / 2.0f))     the code for spawning the recipes with this object in the middle
                GameObject newRecipe = Instantiate(recipePrefab, num, transform.rotation) as GameObject;
                newRecipe.transform.SetParent(gameObject.transform, false);
                newRecipe.GetComponent<Recipe>().Initialize(recipe, false);
                position++;
                //calculates how far you can scroll the recipe list
                maxScrollDistance += recipeYSize + recipeYSpacing;
            }
        }
        screenScale = Screen.height / 1080;
    }

    void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") != 0)
        {
            Scroll(Input.GetAxisRaw("Mouse ScrollWheel") < 0);
        }
        else if (Input.GetAxisRaw("Vertical") != 0)
        {
            Scroll(Input.GetAxisRaw("Vertical") < 0);
        }
    }

    void Scroll(bool up)
    {
        if(displayRecipes )
        {
            int moveDir = 1;
            if (!up)
            {
                moveDir = -1;
            }
            Vector3 newPos = transform.position;
            newPos.y += scrollSensitivity * Time.deltaTime * moveDir * screenScale;
            if(newPos.y < minScrollDistance) //only scroll if it's valid
            {
                newPos.y = minScrollDistance;
            }
            else if (newPos.y > maxScrollDistance) //only scroll if it's valid
            {
                newPos.y = maxScrollDistance;
            }
            transform.position = newPos;
        }
    }

    public void SetRecipe(GameObject recipe)
    {
        currentRecipe = recipe;
    }
    public GameObject GetRecipe()
    {
        return currentRecipe;
    }
    
    public void SetDisplayRecipes(bool recipes)
    {
        displayRecipes = recipes;
        if(displayRecipes) //makes the recipes display
        {
            deliveryMethodParent.SetAsFirstSibling();
            transform.SetAsLastSibling();
            headerText.text = recipeHeader;
        }
        else //makes the delivery methods display
        {
            transform.SetAsFirstSibling();
            deliveryMethodParent.SetAsLastSibling();
            headerText.text = deliveryHeader;
        }
    }
}
