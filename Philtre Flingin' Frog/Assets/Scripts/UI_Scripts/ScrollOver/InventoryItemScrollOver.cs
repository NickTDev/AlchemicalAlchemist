using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemScrollOver : ScrollOverInteraction
{

    public void Click()
    {
        DeactivateText();
    }

    public override void MouseOver()
    {
        ScaleUp();
        Highlight();
        ActivateText();
        transform.SetAsLastSibling(); //to ensure the text box appears in front of other items
    }
    public override void MouseOff()
    {
        ScaleDown();
        RemoveHighlight();
        DeactivateText();
    }
    public void WhileSelected()
    {
        DeactivateText();
        //because the mouse isn't perfectly aligned
        ScaleUp();
        RemoveHighlight();
    }
}
