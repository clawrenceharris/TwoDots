using System;
using System.Collections.Generic;

public class NormalDotPresenter : DotPresenter
{
    public NormalDotPresenter(Dot dot, DotView view) : base(dot, view)
    {
    }

    // public void Connect(IDotPresenter dot)
    // {
    //     if (TryGetPresenter(out ConnectableDotPresenter presenter))
    //     {
    //         presenter.Connect(dot);
    //     }
    // }

    // public void Deselect()
    // {
    //     if (TryGetPresenter(out ConnectableDotPresenter presenter))
    //     {
    //         presenter.Deselect();
    //     }
    // }

    // public void Disconnect()
    // {
    //     if (TryGetPresenter(out ConnectableDotPresenter presenter))
    //     {
    //         presenter.Disconnect();
    //     }
    // }

    // public void Hit()
    // {   
    //     if (TryGetPresenter(out HittablePresenter presenter))
    //     {
    //         presenter.Hit();
    //     }
    // }

    // public void Select()
    // {
    //     if (TryGetPresenter(out ConnectableDotPresenter presenter))
    //     {
    //         presenter.Select();
    //     }
    // }
   
}