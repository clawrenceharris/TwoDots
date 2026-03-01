using System;

public class TilePresenter : ITilePresenter
{
    private TileModel _model;
    private TileView _view;
    public TileModel Model => _model;
    public TileView View => _view;
    public event Action<ITilePresenter> OnTileRemoved;
    public event Action<ITilePresenter> OnTileSpawned;
    public TilePresenter(TileModel model, TileView view)
    {
        _model = model;
        _view = view;
    }
    public void Clear()
    {
        OnTileRemoved?.Invoke(this);
    }
    public void Spawn()
    {
        OnTileSpawned?.Invoke(this);
    }
}