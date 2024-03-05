using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactDisplayScrollover : ScrollOverInteraction
{
    [SerializeField] TeleportationCollectible.teleportIngredient ingredient;
    [SerializeField] Color uncollectedColor; //which color this sprite displays as if not yet collected
    bool isCollected = false;

    protected override void UniqueStart()
    {
        if(GameManager.GetPlayer()!= null)
        {
            isCollected = GameManager.GetPlayer().GetComponent<PlayerInventory>().HasTeleportIngredient((int)ingredient);
        }
        if(!isCollected)
        {
            GetComponent<Image>().color = uncollectedColor; //only see silhouette
            hoverTextStrings[1] = "?????????????????????"; //displays question marks for flavor text if not collected
        }
    }
}
