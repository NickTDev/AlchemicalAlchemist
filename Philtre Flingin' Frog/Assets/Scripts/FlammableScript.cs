using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlammableScript : MonoBehaviour
{
    [SerializeField] int flameDamage;

    public int GetFlameDamage()
    {
        return flameDamage;
    }
    public void SetFlameDamage(int amount)
    {
        flameDamage = amount;
    }
    public void AddFlameDamage(int amount)
    {
        flameDamage += amount;
    }
}
