using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScrollOverInteraction : MonoBehaviour
{
    [SerializeField] bool useHighlight = true; //whether the image highlights when you scrollover
    [SerializeField] bool useScale = true; //whether the image scales up when you scrollover
    [SerializeField] bool useText = true; //whether the image displays hovertext when you scrollover

    [SerializeField] protected float hoverScale; //how large the object grows when you mouse over
    [SerializeField] protected string[] hoverTextStrings; //the text that displays when you mouse over
    [SerializeField] protected Image hoverTextBackground; //the background box for the text
    [SerializeField] protected TMP_Text[] hoverTexts; //array in case of multiple styles of text
    protected float baseScale; //the scale to return to
    protected Image highlight;
    bool moveableItem = false;
    [SerializeField] FMODUnity.EventReference interactSFX;

    void Start()
    {
        UniqueStart();
        if (useHighlight)
        {
            InitializeHighlight();
        }
        if (useScale)
        {
            InitializeScale();
        }
        if (useText)
        {
            IntitializeText();
        }
    }

    public virtual void MouseOver()
    {
        if (useHighlight)
        {
            ScaleUp();
        }
        if (useScale)
        {
            Highlight();
        }
        if (useText)
        {
            ActivateText();
        }
        AudioEngineManager.PlaySound(interactSFX, 1f);
    }
    public virtual void MouseOff()
    {
        if (useHighlight)
        {
            ScaleDown();
        }
        if (useScale)
        {
            RemoveHighlight();
        }
        if (useText)
        {
            DeactivateText();
        }
    }


    protected virtual void InitializeScale()
    {
        if(!moveableItem)
        {
            baseScale = transform.localScale.x;
            hoverScale *= baseScale;
        }
    }
    public void InitializeMoveableScale(ScrollOverInteraction baseScript) //ingredients that you select and move around wonk out a lot
    {
        baseScale = baseScript.GetBaseScale();
        hoverScale = baseScript.GetHoverScale();
        moveableItem = true;
    }

    protected virtual void ScaleUp() //gets slightly larger if you mouse over
    {
        transform.localScale = new Vector3(hoverScale, hoverScale, hoverScale);
    }
    protected virtual void ScaleDown() //removes the mouse over size
    {
        transform.localScale = new Vector3(baseScale, baseScale, baseScale);
    }

    protected virtual void InitializeHighlight()
    {
        highlight = GetComponent<Image>();
        highlight.color = ColorPaletteManager.highlightColor;
        //if its a potion, sets the correct highlight shape
        InventoryItemPotion potionScript = GetComponent<InventoryItemPotion>();
        if (potionScript != null)
        {
            highlight.sprite = potionScript.GetBottleGlow();
        }
    }

    protected void Highlight()
    {
        highlight.enabled = true;
    }
    protected void RemoveHighlight()
    {
        highlight.enabled = false;
    }

    protected void IntitializeText()
    {
        if(hoverTextBackground == null)
        {
            hoverTextBackground = transform.GetChild(0).GetComponent<Image>();
        }
        if(hoverTextStrings.Length != 0 && hoverTextStrings[0] != "")
        {
            for(int i = 0; i < hoverTextStrings.Length; i++)
            {
                hoverTexts[i].text = hoverTextStrings[i];
            }
        }
    }
    protected void ActivateText()
    {
        hoverTextBackground.enabled = true;
        foreach(TMP_Text text in hoverTexts)
        {
            text.enabled = true;
        }
    }
    protected void DeactivateText()
    {
        hoverTextBackground.enabled = false;
        foreach (TMP_Text text in hoverTexts)
        {
            text.enabled = false;
        }
    }
    public void SetTexts(string[] texts)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            hoverTextStrings[i] = texts[i];
        }
    }
    
    //potions in the hotkey
    public void HotkeySelection()
    {
        ScaleUp();
        Highlight();
    }
    public void HotkeyDeselection()
    {
        ScaleDown();
        RemoveHighlight();
    }

    //getters
    public float GetHoverScale()
    {
        return hoverScale;
    }
    public float GetBaseScale()
    {
        return baseScale;
    }


    protected virtual void UniqueStart() { } //in case a child needs to do something unique on start

}
