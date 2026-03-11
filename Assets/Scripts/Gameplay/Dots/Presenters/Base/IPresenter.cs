using UnityEngine;

public interface IPresenter
{
    IBoardEntity GetEntity();
    EntityView GetView();
}