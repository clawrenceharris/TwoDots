using UnityEngine;

/// <summary>
/// A model associated with a one-sided block tile that can be hit.
/// </summary>
public class HittableOneSidedBlock : HittableTile
{
    private readonly Directional _directional;
    /// <summary>
    /// Initializes a new instance of the <see cref="HittableOneSidedBlock"/> class.
    /// This constructor also adds a directional model to the entity.
    /// </summary>
    /// <param name="entity">The board entity</param>
    /// <param name="direction">The direction the block is facing</param>
    /// <param name="hitMax">The maximum times the model can be hit. Defaults to 1</param>
    /// <param name="hitCount">The number of times the model has been hit. Defaults to 0</param>
    public HittableOneSidedBlock(BoardEntity entity, Vector2Int direction, int hitMax = 1, int hitCount = 0) : base(entity, hitMax, hitCount)
    {
        _directional = new Directional(entity, direction);
        _entity.AddModel(_directional);
    }
    public override bool ShouldHit()
    {
        if (!ServiceProvider.Instance.TryGetService<ConnectionService>(out var connectionService)) return false;
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return false;
        return new OneSidedBlockHitRule().CanHit(boardService.BoardPresenter, connectionService.ActiveConnection, _entity.ID);
    }
}