using System;
using DG.Tweening;
/// <summary>
/// Interface for a presenter of a dot.
/// </summary>
public interface IDotPresenter
{
    Dot Dot { get; }
    DotView View { get; }
    event Action<string> OnDotDropped;
    void Initialize(IBoardPresenter board);
    Sequence Spawn();
    bool TryGetPresenter<T>(out T presenter) where T : class, IPresenter;
    T GetPresenter<T>() where T : class, IPresenter;
    void AddPresenter<T>(T presenter) where T : class, IPresenter;
    void RemovePresenter<T>() where T : class, IPresenter;
    void Drop(int targetRow);
}