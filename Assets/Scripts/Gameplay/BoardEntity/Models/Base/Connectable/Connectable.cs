public class Connectable : ModelBase, IConnectable
{
    public Connectable(BoardEntity entity) : base(entity)
    {
    }

    public bool IsConnected => throw new System.NotImplementedException();

    public bool CanConnect(IBoardPresenter board, ConnectionSession connectionSession, string id)
    {
        throw new System.NotImplementedException();
    }

    public void Connect()
    {
        throw new System.NotImplementedException();
    }

    public void Disconnect()
    {
        throw new System.NotImplementedException();
    }
}