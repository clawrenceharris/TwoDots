using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DotsRenderer))]
public class SpriteController : MonoBehaviour
{
    [SerializeField]private int baseSortingOrder = 0;
    protected DotsRenderer _renderer;
    
    public void Awake(){
        _renderer = GetComponent<DotsRenderer>();
    }

    private void Start(){
        SetSortingOrder();
    }
    private void SetSortingOrder()
    {
        foreach (var target in _renderer.Targets)
        {
            foreach (var sprite in target.Renderers)
                sprite.sortingOrder += baseSortingOrder;
        }
    }

   

    public void BringSpriteToTop()
    {
         foreach (var target in _renderer.Targets)
        {
            foreach (var sprite in target.Renderers)
                sprite.sortingOrder += 100;
        }
    }

    public void BringSpriteBack(){
        foreach (var target in _renderer.Targets)
        {
            foreach (var sprite in target.Renderers)
                sprite.sortingOrder -= 100;
        }
    }
}
