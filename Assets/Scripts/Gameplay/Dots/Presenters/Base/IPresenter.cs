using UnityEngine;

public interface IPresenter
{
    IBoardEntity Entity { get; }
    EntityView View { get; }
    void Initialize(IBoardPresenter board);
}