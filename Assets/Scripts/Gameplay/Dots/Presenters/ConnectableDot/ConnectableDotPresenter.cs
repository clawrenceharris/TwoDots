
using System;
using System.Collections.Generic;
using System.Linq;

public class ConnectableDotPresenter : DotPresenter, IConnectableDotPresenter
{
    private readonly DotSelectionFeedback _selectionFeedback;
    private readonly ColorableModel _colorable;
    public ConnectableDotPresenter(Dot dot, DotView view) : base(dot, view)
    {
        _selectionFeedback = view.GetComponent<DotSelectionFeedback>();
        _colorable = dot.TryGetModel(out ColorableModel colorable) ? colorable : null;
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