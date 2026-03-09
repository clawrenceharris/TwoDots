using UnityEngine;

[CreateAssetMenu(fileName = "DotColorScheme", menuName = "Scriptable Objects/Dot Color Scheme")]
public class DotColorScheme : ScriptableObject
{
    public Color baseColor;
    public Color accentColor;
    public Color detailColor;
}