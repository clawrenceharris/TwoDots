
public class ConnectableDotPresenter : DotPresenter, IConnectableDotPresenter
{
    private readonly DotSelectionFeedback _selectionFeedback;
    private readonly ColorableModel _colorable;
    public ConnectableDotPresenter(Dot dot, DotView view) : base(dot, view)
    {
        _selectionFeedback = view.GetComponent<DotSelectionFeedback>();
        _colorable = dot.TryGetModel(out ColorableModel colorable) ? colorable : null;

    }

    public void Connect(IConnectionModel connection)
    {
        _selectionFeedback.PlaySelectionAnimation(ColorSchemeService.FromDotColor(connection.CurrentColor));
        connection.OnColorChanged += OnColorChanged;
        connection.OnConnectionCompleted += OnConnectionCompleted;
        connection.OnDotRemovedFromPath += OnDotRemovedFromPath;
    }

    public void Select(ConnectionContext context)
    {
        var dotColor = _colorable.GetComparableColor(context.ConnectionColor);
        _selectionFeedback.PlaySelectionAnimation(ColorSchemeService.FromDotColor(dotColor));
    }

   

    private void OnDotRemovedFromPath(string dotId)
    {
        if (dotId != _dot.ID) return;
        Disconnect();
    }

    private void OnConnectionCompleted(ConnectionContext payload)
    {
        Disconnect();
    }

    private void OnColorChanged(DotColor color)
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