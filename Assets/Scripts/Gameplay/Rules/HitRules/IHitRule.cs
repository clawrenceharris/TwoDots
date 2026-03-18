/// <summary>
/// A rule that determines if a hittable should be hit.
/// </summary>
public interface IHitRule
{
    /// <summary>
    /// Checks if the hittable entity should be hit.
    /// </summary>
    /// <param name="board">The board presenter</param>
    /// <param name="connection">The connection</param>
    /// <param name="hittableId">The hittable id. Must have a hittable model attached to be considered.</param>
    /// <returns>True if the hittable entity should be hit, false otherwise</returns>
    bool CanHit(IBoardPresenter board, Connection connection, string hittableId);
}