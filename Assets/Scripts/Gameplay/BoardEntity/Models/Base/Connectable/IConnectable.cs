public interface IConnectable
{
    bool CanConnect(IBoardPresenter board, ConnectionSession connectionSession, string id);

    bool IsConnected { get; }
    void Connect();
    void Disconnect();
    
}