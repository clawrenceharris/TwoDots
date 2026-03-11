using UnityEngine;

public class ConnectionService : MonoBehaviour
{
    private IConnectionPresenter _connectionPresenter;
    
    public void Initialize(IBoardPresenter board)
    {
        _connectionPresenter = new ConnectionPresenter();
        _connectionPresenter.Initialize(board);
    }

    public ConnectionSession ActiveConnection
    {
        get
        {
            if(_connectionPresenter == null) return null;
            return _connectionPresenter.Session;
        }

    }
    public ConnectionResult LastConnection {
        get
        {
            if(_connectionPresenter == null) return null;
            return _connectionPresenter.ConnectionHistory.TryPeek(out var result) ? result : null;
        }
    }

}
