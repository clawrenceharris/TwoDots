using UnityEngine;

public class DirectionalPresenter : EntityPresenter, IDirectionalPresenter
{
    public Directional Directional => _entity.GetModel<Directional>();
    public DirectionalPresenter(BoardEntity entity, EntityView view) : base(entity, view)
    {
    }
    public override void Initialize()
    {
        base.Initialize();

        View.transform.rotation = Quaternion.Euler(Directional.ToRotation(Directional.FacingDirection.x, Directional.FacingDirection.y));
    }
}