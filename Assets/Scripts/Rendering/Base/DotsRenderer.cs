using System;
using UnityEngine;

public class DotsRenderer : MonoBehaviour
{
    [Serializable]
    public class Target
    {
        public SkinColorRole Role = SkinColorRole.Base;
        public SpriteRenderer[] Renderers;
    }
    [SerializeField]private Material _hitMaterial;
    [SerializeField] private SpriteRenderer _fallbackBaseRenderer;
    [SerializeField] private Material _defaultMaterial;
    public Material HitMaterial
    {
        get =>  _hitMaterial;
    }
    [SerializeField] private Target[] _targets;

    public SpriteMaskInteraction MaskInteraction
    {
        get { return _fallbackBaseRenderer.maskInteraction; }
        set {
            foreach (var target in _targets)
            {
                foreach (var renderer in target.Renderers)
                {
                    renderer.maskInteraction = value;
                }
            }
            _fallbackBaseRenderer.maskInteraction = value;
        }
    }

    public Target[] Targets => _targets;
    public SpriteRenderer BaseRenderer => _fallbackBaseRenderer;

    public Material DefaultMaterial => _defaultMaterial;

    private void Awake()
    {
        if (_fallbackBaseRenderer == null)
            _fallbackBaseRenderer = GetComponent<SpriteRenderer>();
    }

    public void ApplySkin(Skin skin)
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
    public void SetMaterial(Material material)
    {
        if (_targets != null)
        {
            foreach (var target in _targets)
            {
                foreach (var renderer in target.Renderers)
                {
                    renderer.material = material;
                }
            }
        }
        if (_fallbackBaseRenderer != null)
        {
            _fallbackBaseRenderer.material = material;
        }
    }
    public void SetColor(Color color)
    {
        if (_targets != null)
        {
            for (int i = 0; i < _targets.Length; i++)
            {
                var binding = _targets[i];
                foreach (var sprite in binding.Renderers)
                {
                    sprite.color = color;
                }
            }
        
        }
        if (_fallbackBaseRenderer != null)
        {
            _fallbackBaseRenderer.color = color;
        }
    }
}
