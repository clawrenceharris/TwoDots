using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ColorScheme", menuName = "Two Dots/Color Scheme")]
public class ColorScheme : ScriptableObject
{
    public Color purple;
    public Color yellow;
    public Color blue;
    public Color green;
    public Color red;
    public Color blank;
    public Color backgroundColor;


    public SkinColorScheme anchor;
    public SkinColorScheme clock;
    public SkinColorScheme bomb;
    public SkinColorScheme nesting;
    public SkinColorScheme beetle;
    public SkinColorScheme lotus;

    public SkinColorScheme emptyTile;
}
