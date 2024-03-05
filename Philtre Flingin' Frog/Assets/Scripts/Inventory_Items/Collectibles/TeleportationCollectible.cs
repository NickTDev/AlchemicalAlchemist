using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationCollectible : Collectible
{
    [SerializeField] teleportIngredient ingredient;

    public enum teleportIngredient
    {
        ANATHILAFKEN_SOIL,
        GHOSTLY_FEATHER,
        ZORKORKLE_TEAR,
        NUM_ARTIFACTS
    }

    public override void Collect()
    {
        GameManager.GetPlayer().GetComponent<PlayerInventory>().CollectTeleportIngredient((int)ingredient);
        HandleCollection();
    }
}
