using UnityEngine;

public class ConnectionService : MonoBehaviour
{
    private readonly IConnectionPresenter _connectionPresenter = new ConnectionPresenter();
    
    public void Initialize(IBoardPresenter board)
    {
        _connectionPresenter.Initialize(board);
    }

    public Connection ActiveConnection
    {
        get
        {
            if(_connectionPresenter == null) return null;
            return _connectionPresenter.Connection;
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
