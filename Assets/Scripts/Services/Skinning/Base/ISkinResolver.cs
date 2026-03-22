
public interface ISkinResolver<T> where T : class,IBoardEntity
{
    Skin? ResolveSkin(T entity);
}