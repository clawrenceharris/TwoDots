using UnityEngine;

public class TileView : MonoBehaviour
{
    private TileModel _model;

    public string ID => _model.ID;
    public TileType Type => _model.TileType;

    public void Init(TileModel model)
    {
        _model = model;
    }
}