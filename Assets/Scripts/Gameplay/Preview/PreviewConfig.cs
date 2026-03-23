using UnityEngine;

[CreateAssetMenu(menuName = "TwoDots/Preview/Preview Config", fileName = "PreviewConfig")]
public class PreviewConfig : ScriptableObject
{
    [Header("Global")]
    public bool EnablePreview = true;
    public bool EnableAudioBridge = true;

    [Header("Mechanics")]
    public bool EnableNestingPreview = true;
    public bool EnableBeetlePreview = true;

    [Header("Animation")]
    [Min(0f)] public float ShakeDuration = 0.2f;
    [Min(0f)] public float PulseDuration = 0.3f;
    [Min(0f)] public float WingFlapDuration = 0.18f;
}
