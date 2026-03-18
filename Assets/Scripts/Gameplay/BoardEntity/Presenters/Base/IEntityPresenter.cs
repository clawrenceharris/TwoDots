using UnityEngine;

public interface IEntityPresenter
{
   
    bool TryGetPresenter<T>(out T presenter) where T : class, IPresenter;
    T GetPresenter<T>() where T : class, IPresenter;
    void AddPresenter<T>(T presenter) where T : class, IPresenter;
    void RemovePresenter<T>() where T : class, IPresenter;
}