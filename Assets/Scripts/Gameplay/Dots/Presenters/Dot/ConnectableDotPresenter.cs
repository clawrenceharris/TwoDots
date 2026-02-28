using System;

public class ConnectableDotPresenter : DotPresenter, IConnectableDotPresenter
{
    private readonly DotSelectionFeedback _selectionFeedback;
    private readonly ColorableModel _colorable;
    public ConnectableDotPresenter(Dot dot, DotView view) : base(dot, view)
    {
        _selectionFeedback = view.GetComponent<DotSelectionFeedback>();
        _colorable = dot.TryGetComponent(out ColorableModel colorable) ? colorable : null;

    }

    public void Connect(IConnectionModel connection)
    {
        _selectionFeedback.PlaySelectionAnimation(ColorSchemeService.FromDotColor(connection.CurrentColor));
        connection.OnColorChanged += OnColorChanged;
        connection.OnConnectionCompleted += OnConnectionCompleted;
        connection.OnDotRemovedFromPath += OnDotRemovedFromPath;
    }

    private void OnDotRemovedFromPath(string dotId)
    {
        if (dotId != _dot.ID) return;
        Disconnect();
    }

    private void OnConnectionCompleted(ConnectionCompletedPayload payload)
    {
        Disconnect();
    }

    private void OnColorChanged(DotColor color)
    {
        _selectionFeedback.ChangeFillColor(ColorSchemeService.FromDotColor(color));
    }
    
    public void Disconnect()
    {
        _selectionFeedback.PlayDeselectionAnimation();
    }

}