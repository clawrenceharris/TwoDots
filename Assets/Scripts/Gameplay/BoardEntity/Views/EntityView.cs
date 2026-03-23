using UnityEngine;

public class EntityView : MonoBehaviour
{
    public IDotVisuals Visuals { get; protected set; }
    private DotsRenderer _renderer;
    public DotsRenderer Renderer => _renderer;
    public T GetVisuals<T>() where T : IDotVisuals
    {
        TryGetComponent(out T visuals);
        Visuals = visuals;
        return visuals;
    }
    private void Awake()
    {
        TryGetComponent(out _renderer);
        _renderer.MaskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

}