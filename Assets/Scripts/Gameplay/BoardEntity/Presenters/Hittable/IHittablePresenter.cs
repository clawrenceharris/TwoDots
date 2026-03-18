using DG.Tweening;
using UnityEngine;


/// <summary>
/// Interface for a presenter of a dot that can be hit.
/// </summary>
public interface IHittablePresenter : IPresenter
{
    IClearablePresenter Clearable { get; }

    Sequence Hit();
}