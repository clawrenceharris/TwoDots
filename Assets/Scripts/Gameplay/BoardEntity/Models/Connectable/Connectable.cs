/// <summary>
/// A model associated with a board entity that can be connected to other dots.
/// </summary>
public class Connectable : ModelBase, IConnectable
{
    public Connectable(BoardEntity entity) : base(entity)
    {
    }

    public bool IsConnected { get; private set; }

    public bool CanConnect(string fromDotId)
    {
        var rule = new BaseConnectionRule();
        var board = ServiceProvider.Instance.GetService<BoardService>().BoardPresenter;
        var connectionSession = ServiceProvider.Instance.GetService<ConnectionService>().ActiveConnection;
        return rule.CanConnect(fromDotId, _entity.ID, connectionSession, board);
    }

    public void Connect()
    {
        IsConnected = true;
    }

    public void Disconnect()
    {
        IsConnected = false;
    }
}