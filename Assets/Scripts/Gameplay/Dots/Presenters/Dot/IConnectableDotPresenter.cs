/// <summary>
/// Interface for a presenter of a dot that can be connected to a connection.
/// </summary>
public interface IConnectableDotPresenter
{
    void Connect(IConnectionModel connection);
    void Disconnect();

}