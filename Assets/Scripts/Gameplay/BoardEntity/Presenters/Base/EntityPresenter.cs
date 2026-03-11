using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityPresenter : IEntityPresenter
{
    protected BoardEntity _entity;
    protected EntityView _view;
    public virtual IBoardEntity GetEntity() => _entity;
    public virtual EntityView GetView() => _view;
    protected IBoardPresenter _board;
    private readonly Dictionary<Type, IPresenter> _presenters = new();
    public EntityPresenter(BoardEntity entity, EntityView view)
    {
        _entity = entity;
        _view = view;
    }

    public virtual void Initialize(IBoardPresenter board)
    {
        _board = board;
        
    }
    public void AddPresenter<T>(T presenter) where T : class, IPresenter
    {
        _presenters.Add(typeof(T), presenter);
    }

    public void RemovePresenter<T>() where T : class, IPresenter
    {
        _presenters.Remove(typeof(T));
    }
    public T GetPresenter<T>() where T : class, IPresenter
    {
        if (TryGetPresenter(out T presenter))
        {
            return presenter;
        }
        return null;
    }


    public bool TryGetPresenter<T>(out T presenter) where T : class, IPresenter
    {

        if (_presenters.TryGetValue(typeof(T), out IPresenter tPresenter))
        {
            presenter = tPresenter as T;
            return true;
        }
        else
        {
            foreach (var kvp in _presenters)
            {
                if (kvp.Value is T tPresenterValue)
                {
                    presenter = tPresenterValue;
                    return true;
                }
            }
        }
        presenter = null;
        return false;
    }
}