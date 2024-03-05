using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

public class InventorySlotScript : MonoBehaviour
{
    private int slotIndex;

    public void Initialize(int num)
    {
        slotIndex = num;
        name += slotIndex;
    }
    public int GetSlotIndex()
    {
        return slotIndex;
    }
}
