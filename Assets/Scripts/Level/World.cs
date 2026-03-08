using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "World", menuName = "Two Dots/World" )]
public class World : ScriptableObject
{
    public TextAsset[] levels;
    public ColorScheme colorScheme; 
}
