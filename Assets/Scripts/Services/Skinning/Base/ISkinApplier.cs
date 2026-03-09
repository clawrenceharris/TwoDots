using UnityEngine;

public interface ISkinApplier<T> where T : MonoBehaviour
{
    void Apply(T entity, Skin skin);
}



