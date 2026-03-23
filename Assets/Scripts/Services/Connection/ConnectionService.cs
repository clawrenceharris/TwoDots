using UnityEngine;

public class ConnectionService : MonoBehaviour
{
    private readonly IConnectionPresenter _connectionPresenter = new ConnectionPresenter();
    
    public void Initialize(IBoardPresenter board)
    {
        _connectionPresenter.Initialize(board);
    }

    public Connection ActiveConnection =>_connectionPresenter.Connection;

}
