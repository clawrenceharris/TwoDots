using System;
using UnityEngine;

public class DotRenderer : MonoBehaviour
{
    [Serializable]
    public class Target
    {
        public SkinColorRole Role = SkinColorRole.Base;
        public SpriteRenderer[] Renderers;
    }

    [SerializeField] private SpriteRenderer _fallbackBaseRenderer;
    [SerializeField] private Target[] _targets;
    public Target[] Targets => _targets;

    private void Awake()
    {
        if (_fallbackBaseRenderer == null)
            _fallbackBaseRenderer = GetComponent<SpriteRenderer>();
    }

    public void ApplySkin(DotSkin skin)
    {
        ApplyRoleColor(SkinColorRole.Base, skin.BaseColor);
        ApplyRoleColor(SkinColorRole.Accent, skin.AccentColor);
        ApplyRoleColor(SkinColorRole.Detail, skin.DetailColor);
    }

    private void ApplyRoleColor(SkinColorRole role, Color color)
    {
        bool applied = false;
        if (_targets != null)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                var binding = _targets[i];
                if (binding == null || binding.Role != role || binding.Renderers == null) continue;

                for (int j = 0; j < binding.Renderers.Length; j++)
                {
                    if (binding.Renderers[j] == null) continue;
                    binding.Renderers[j].color = color;
                    applied = true;
                }
            }
        }

        if (!applied && role == SkinColorRole.Base && _fallbackBaseRenderer != null)
            _fallbackBaseRenderer.color = color;
    }
}
