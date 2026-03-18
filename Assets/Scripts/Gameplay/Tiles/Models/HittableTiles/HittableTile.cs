/// <summary>
/// A model associated with a tile that can be hit.
/// </summary>
public class HittableTile : Hittable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HittableTile"/> class.
    /// </summary>
    /// <param name="entity">The board entity</param>
    /// <param name="hitMax">The maximum times the model can be hit. Defaults to 1</param>
    /// <param name="hitCount">The number of times the model has been hit. Defaults to 0</param>
    public HittableTile(BoardEntity entity, int hitMax = 1, int hitCount = 0) : base(entity, hitMax, hitCount)
    {
    }
    public HittableTile(BoardEntity entity, Clearable clearable, int hitMax = 1, int hitCount = 0) : base(entity, clearable, hitMax, hitCount)
    {
    }
    public override bool ShouldHit()
    {
        if (!ServiceProvider.Instance.TryGetService<ConnectionService>(out var connectionService)) return false;
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return false;
        return new TileHitRule().CanHit(boardService.BoardPresenter, connectionService.ActiveConnection, _entity.ID);
    }
}