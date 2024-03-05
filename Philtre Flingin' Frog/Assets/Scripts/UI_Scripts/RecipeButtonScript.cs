using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeButtonScript : MonoBehaviour
{
    [SerializeField] GameObject recipeSpawner;
    [SerializeField] GameObject recipeVisual;
    [SerializeField] float hoverScale; //how large the recipe grows when you mouse over
    float baseScale; //the scale to return to

    void Start()
    {
        baseScale = recipeVisual.transform.localScale.x;
        hoverScale *= baseScale;
    }
    public void Click()
    {
        RecipeSpawnerScript recipeSpawnerScript = recipeSpawner.GetComponent<RecipeSpawnerScript>();
        GameObject recipe = recipeSpawnerScript.GetRecipe();
        if(recipe != null)
        {
            recipe.GetComponent<Recipe>().CheckForBrew();
        }
    }

    public void MouseOver() //gets slightly larger if you mouse over when available to click
    {
        RecipeSpawnerScript recipeSpawnerScript = recipeSpawner.GetComponent<RecipeSpawnerScript>();
        GameObject recipe = recipeSpawnerScript.GetRecipe();
        if (recipe != null && recipe.GetComponent<Recipe>().GetIsFilled())
        {
            recipeVisual.transform.localScale = new Vector3(hoverScale, hoverScale, hoverScale);
        }
    }
    public void MouseOff() //removes the mouse over size
    {
        recipeVisual.transform.localScale = new Vector3(baseScale, baseScale, baseScale);
    }
}
