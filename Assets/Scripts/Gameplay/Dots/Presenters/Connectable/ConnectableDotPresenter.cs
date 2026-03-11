
using System;
using System.Collections.Generic;
using System.Linq;

public class ConnectableDotPresenter :EntityPresenter,  IConnectableDotPresenter
{
    private readonly DotSelectionFeedback _selectionFeedback;
    private readonly Colorable _colorable;
    public Dot Dot => _entity as Dot;
    public ConnectableDotPresenter(Dot dot, DotView view)
    : base(dot, view)
    {
        _selectionFeedback = view.GetComponent<DotSelectionFeedback>();
        _colorable = dot.TryGetModel(out Colorable colorable) ? colorable : null;
    }

    

    public void Connect(DotColor connectionColor)
    {
        _selectionFeedback.PlaySelectionAnimation(ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(connectionColor));
        
    }

    public void Select(ConnectionSession session)
    {
        var dotColor = _colorable.GetComparableColor(session.Color);
        _selectionFeedback.PlaySelectionAnimation(ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(dotColor));
    }


    public void ChangeColor(DotColor color)
    {
        
        _selectionFeedback.SetFillColor(ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(color));
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