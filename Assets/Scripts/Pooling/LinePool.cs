using System.Collections.Generic;
using UnityEngine;

public class LinePool : Pool
{
    [SerializeField] private ConnectorLineView _linePrefab;
    private readonly Queue<ConnectorLineView> _linePool = new();

    public override void Fill(int size)
    {
        Clear(_linePool);
        for (int i = 0; i < size; i++)
        {
            ConnectorLineView line = Instantiate(_linePrefab, Vector2.one, Quaternion.identity);
            line.transform.parent = transform;

            _linePool.Enqueue(line);

            line.gameObject.SetActive(false);

        }
    }

    public override T Get<T>()
    {


        if (_linePool.Count == 0)
        {
            Debug.LogWarning("Line pool is empty!");
            return null;
        }
        ConnectorLineView line = _linePool.Dequeue();
        if(line == null) {
            Debug.LogWarning("Line is null!");
            return null;
        }
        Debug.Log("Line: " + line);
        line.gameObject.SetActive(true);
        return line as T;

    }


    public override void Return<T>(T line)
    {
        _linePool.Enqueue(line as ConnectorLineView);
        line.gameObject.SetActive(false);
        line.transform.SetParent(transform);
    }
    
    public override void Clear<T>(Queue<T> pool)
    {
        base.Clear(_linePool);
    }

    
}
