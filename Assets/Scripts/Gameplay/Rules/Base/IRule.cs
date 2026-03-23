/// <summary>
/// A rule that determines if an entity should be hit or cleared.
/// </summary>
public interface IRule
{
    /// <summary>
    /// Checks if the entity should be hit or cleared.
    /// </summary>
    /// <param name="board">The board presenter</param>
    /// <param name="connection">The connection</param>
    /// <param name="hittableId">The entity id.</param>
    /// <returns>True if the entity should be hit, false otherwise</returns>
    bool CanHit(IBoardPresenter board, Connection connection, string entityId);
}