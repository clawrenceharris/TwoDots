using UnityEngine;

public class BasePresenter
{

    private readonly BoardEntity _entity;
    private readonly EntityView _view;
    public BoardEntity Entity => _entity;
    public EntityView View => _view;

    public BasePresenter(BoardEntity entity, EntityView view)
    {
        _entity = entity;
        _view = view;
    }

}