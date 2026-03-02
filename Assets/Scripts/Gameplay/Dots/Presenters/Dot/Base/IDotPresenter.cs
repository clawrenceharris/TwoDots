using System;
using DG.Tweening;
/// <summary>
/// Interface for a presenter of a dot.
/// </summary>
public interface IDotPresenter
{
    Dot Dot { get; }
    DotView View { get; }
    event Action<IDotPresenter> OnDotCleared;
    event Action<IDotPresenter> OnDotDropped;

    Sequence Clear();
    Sequence Spawn();
    bool TryGetPresenter<T>(out T presenter) where T : DotPresenter;
    T GetPresenter<T>() where T : DotPresenter;
    void AddPresenter<T>(T presenter) where T : DotPresenter;
    void RemovePresenter<T>() where T : DotPresenter;
    void Drop(int targetRow);
}