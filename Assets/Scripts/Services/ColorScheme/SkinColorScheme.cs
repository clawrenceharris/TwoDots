using UnityEngine;

[CreateAssetMenu(fileName = "SkinColorScheme", menuName = "Two Dots/Skin Color Scheme")]
public class SkinColorScheme : ScriptableObject
{
    public Color baseColor;
    public Color accentColor;
    public Color detailColor;
}