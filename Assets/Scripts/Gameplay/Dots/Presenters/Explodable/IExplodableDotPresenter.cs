using System.Collections.Generic;
using DG.Tweening;

public interface IExplodableDotPresenter : IPresenter
{
    Sequence Explode();
    void PrepareForExplode(List<string> targetHittables, List<string> explodableIds);
}