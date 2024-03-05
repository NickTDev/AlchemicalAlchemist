using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DeliveryMethodUI : ScrollOverInteraction
{
    protected override void InitializeHighlight()
    {
        highlight = GetComponent<Image>();
    }
    public override void MouseOver()
    {
        //ScaleUp();
        //Highlight();
        ActivateText();
    }
    public override void MouseOff()
    {
        //ScaleDown();
        //RemoveHighlight();
        DeactivateText(); 
    }
}
