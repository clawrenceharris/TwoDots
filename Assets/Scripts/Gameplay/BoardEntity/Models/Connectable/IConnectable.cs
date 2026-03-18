public interface IConnectable
{
    bool CanConnect(string fromDotId);

    bool IsConnected { get; }
    void Connect();
    void Disconnect();
    
}