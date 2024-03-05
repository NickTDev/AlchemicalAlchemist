using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPaletteManager : MonoBehaviour
{
    [SerializeField] Color highlightGlowColor; //the color that all highlighted objects should use
    public static Color highlightColor;

    void Awake()
    {
        highlightColor = this.highlightGlowColor;
    }
}
