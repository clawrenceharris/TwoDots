using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public interface IExplodablePresenter : IPresenter
{
    Sequence Explode();
    void PrepareForExplode(List<string> targetHittables, List<string> explodableIds);
}