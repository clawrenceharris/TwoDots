using UnityEngine;

public class EntityView : MonoBehaviour
{
    private DotsRenderer _renderer;
    public DotsRenderer Renderer => _renderer;

    private void Awake()
    {
        TryGetComponent(out _renderer);
        _renderer.MaskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

}