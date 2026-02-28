using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ColorScheme", menuName = "Scriptable Objects/Color Scheme")]
public class ColorScheme : ScriptableObject
{
    public Color purple;
    public Color yellow;
    public Color blue;
    public Color green;
    public Color red;
    public Color blank;
    public Color backgroundColor;

    public Color bombLight;
    public Color bombDark;

    public DotColorScheme anchor;
    public DotColorScheme clock;
    public DotColorScheme bomb;
    public DotColorScheme nesting;
    public DotColorScheme beetle;
    public DotColorScheme lotus;
}
