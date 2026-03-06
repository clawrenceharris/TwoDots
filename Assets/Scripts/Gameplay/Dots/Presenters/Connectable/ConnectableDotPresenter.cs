
using System;
using System.Collections.Generic;
using System.Linq;

public class ConnectableDotPresenter : BasePresenter, IConnectableDotPresenter
{
    private readonly DotSelectionFeedback _selectionFeedback;
    private readonly ColorableDot _colorable;
    public ConnectableDotPresenter(IDotPresenter dot, IBoardPresenter board)
    : base(dot, board)
    {
        _selectionFeedback = dot.View.GetComponent<DotSelectionFeedback>();
        _colorable = dot.Dot.TryGetModel(out ColorableDot colorable) ? colorable : null;
    }

    

    public void Connect(DotColor connectionColor)
    {
        _selectionFeedback.PlaySelectionAnimation(ColorSchemeService.FromDotColor(connectionColor));
        
    }

    public void Select(ConnectionResult context)
    {
        var dotColor = _colorable.GetComparableColor(context.ConnectionColor);
        _selectionFeedback.PlaySelectionAnimation(ColorSchemeService.FromDotColor(dotColor));
    }


    public void ChangeColor(DotColor color)
    {
        
        _selectionFeedback.SetFillColor(ColorSchemeService.FromDotColor(color));
    }

    public void Disconnect()
    {
        _selectionFeedback.PlayDeselectionAnimation();
       
    }

    public void Deselect()
    {
        _selectionFeedback.PlayDeselectionAnimation();
    }
}