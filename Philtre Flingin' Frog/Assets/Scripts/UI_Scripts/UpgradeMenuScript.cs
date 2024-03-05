using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenuScript : MonoBehaviour
{
    [SerializeField] GameObject recipePrefab;
    [SerializeField] GameObject spawnOrigin;
    [SerializeField] float scale;
    [SerializeField] float spacing;
    [SerializeField] int recipeCategories = 4;
    [SerializeField] int recipesPerCategory = 3;
    [SerializeField] int unlockCharges = 1;
    [SerializeField] GameObject exitButton;
    List<Recipe> recipeList = new List<Recipe>();

    void Start() //spawns the recipes
    {
        GameManager.SetPaused(true);

        RecipeManager recipeManager = GameObject.FindWithTag("Player").GetComponent<RecipeManager>();
        float recipeYSize = recipePrefab.GetComponent<RectTransform>().rect.height * scale;
        float recipeXSize = recipePrefab.GetComponent<RectTransform>().rect.width * scale;

        Vector3 recipeScale = new Vector3(scale, scale, scale);
        bool prevUnlocked = false;
        for(int x = 0; x < recipeCategories; x++)
        {
            prevUnlocked = false; //can never unlock level 1 recipes
            for (int y = 0; y < recipesPerCategory; y++)
            {
                GameObject recipe = recipeManager.potionPrefabs[x * recipesPerCategory + y];
                Vector3 pos = new Vector3((recipeXSize + spacing) * (x - (recipeCategories-1) / 2.0f), (recipeYSize + spacing) * -y, 0);
                GameObject newRecipe = Instantiate(recipePrefab, pos, transform.rotation) as GameObject;
                newRecipe.transform.SetParent(spawnOrigin.transform, false);
                newRecipe.transform.localScale = recipeScale;
                //initializing stuff
                Recipe recipeScript = newRecipe.GetComponent<Recipe>();
                recipeScript.Initialize(recipe, true);
                recipeScript.SetUnlockable(prevUnlocked);
                recipeScript.SetUpgradeMenu(this);
                prevUnlocked = recipeScript.GetUnlocked();
                recipeList.Add(recipeScript);
            }
        }
        /*
        int position = 0;
        foreach (GameObject recipe in recipeManager.potionPrefabs)
        {
            Vector3 num = new Vector3(recipeYSize * -position, recipeYSize 0); //((recipeYSpacing + recipeYSize) * (i - (recipeManager.potionPrefabs.Length-1) / 2.0f))     the code for spawning the recipes with this object in the middle
            GameObject newRecipe = Instantiate(recipePrefab, num, transform.rotation) as GameObject;
            newRecipe.transform.SetParent(gameObject.transform, false);
            newRecipe.GetComponent<Recipe>().Initialize(recipe, false);
            position++;
        }
        */
    }

    void OnDestroy()
    {
        GameManager.SetPaused(false);
    }

    public void Unlocked()
    {
        unlockCharges--;
        //if you're out of charges, can't unlock any more
        if(unlockCharges <= 0)
        {
            exitButton.SetActive(true);
            for(int i = 0; i < recipeList.Count; i++)
            {
                recipeList[i].GetComponent<Recipe>().SetUnlockable(false);
            }
        }
    }
}
