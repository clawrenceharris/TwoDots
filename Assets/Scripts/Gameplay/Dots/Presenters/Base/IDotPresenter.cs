using System;
using DG.Tweening;
/// <summary>
/// Interface for a presenter of a dot.
/// </summary>
public interface IDotPresenter
{
    Dot Dot { get; }
    DotView DotView { get; }
    event Action<string> OnDotDropped;
    void Initialize(IBoardPresenter board);
    Sequence Spawn();
   
    Sequence Drop(int targetRow);
}