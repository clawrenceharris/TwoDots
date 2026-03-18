using DG.Tweening;
using UnityEngine;

public interface IClearablePresenter : IPresenter
{
    Sequence Clear();
}