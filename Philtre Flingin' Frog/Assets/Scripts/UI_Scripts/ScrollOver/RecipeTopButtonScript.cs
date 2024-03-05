using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeTopButtonScript : ScrollOverInteraction
{
    [SerializeField] bool favorsRecipes;
    [SerializeField] float selectedScale; //how large the button looks when it's selected
    [SerializeField] bool selected;
    [SerializeField] Color deselectedColor;
    [SerializeField] RecipeTopButtonScript otherButtonScript; //the counterpart button
    Color selectedColor;
    Image buttonImage;
    RecipeSpawnerScript recipeSpawner;

    protected override void InitializeScale()
    {
        //scale
        baseScale = transform.localScale.x;
        hoverScale *= baseScale;
        selectedScale *= baseScale;
        //color
        buttonImage = transform.GetChild(0).GetComponent<Image>();
        selectedColor = buttonImage.color;
        ToggleSelect(selected); //to make sure is the right size if selected
        //misc
        recipeSpawner = FindObjectOfType<RecipeSpawnerScript>().GetComponent<RecipeSpawnerScript>();
    }

    public override void MouseOver()
    {
        if(!selected)
        {
            ScaleUp();
            Highlight();
            //ActivateText();
        }
    }
    public override void MouseOff()
    {
        if (!selected)
        {
            ScaleDown();
            RemoveHighlight();
            //DeactivateText();
        }
    }

    public void Click()
    {
        if(!selected) //no need to repeat if already selected
        {
            ToggleSelect(true);
            recipeSpawner.SetDisplayRecipes(favorsRecipes);
            otherButtonScript.ToggleSelect(false);
        }
    }

    public void ToggleSelect(bool select)
    {
        selected = select;
        RemoveHighlight();
        if (selected)
        {
            buttonImage.color = selectedColor;
            transform.localScale = new Vector3(selectedScale, selectedScale, selectedScale);
        }
        else
        {
            buttonImage.color = deselectedColor;
            transform.localScale = new Vector3(baseScale, baseScale, baseScale);
        }
    }
}
