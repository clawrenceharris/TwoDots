using System.Collections.Generic;
using UnityEngine;

public abstract class Pool : MonoBehaviour
{
    [SerializeField] private int _size;

    public int Size => _size;
    public abstract void Fill(int size);

    public abstract T Get<T>() where T : MonoBehaviour;
    public abstract void Return<T>(T item) where T : MonoBehaviour;
    public virtual void Clear<T>(Queue<T> pool) where T : MonoBehaviour{
        foreach (var item in pool)
        {
            Destroy(item.gameObject);
        }
        pool.Clear();
    }
}