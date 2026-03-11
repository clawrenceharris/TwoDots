public abstract class ModelBase : IModel
{
    protected readonly BoardEntity _entity;
    public ModelBase(BoardEntity entity)
    {
        _entity = entity;
    }
}