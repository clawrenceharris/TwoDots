// using System;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class DotPool : Pool
// {
//        private readonly Queue<DotView> _dotPool = new();


//     public override void Fill(int size)
//     {
        
//     }
   


//     public override T Get<T>()
//     {
//         if (typeof(T) != typeof(DotView))
//         {
//             throw new ArgumentException($"Type {typeof(T)} is not a {typeof(DotView)}");
//         }
      
//         if (_dotPool.Count == 0) return null;
//         DotView dot = _dotPool.Dequeue();
//         dot.gameObject.SetActive(true);
//         return dot as T;
//     }

//     public override void Return<T>(T dot)
//     {
//         if (typeof(T) != typeof(DotView))
//         {
//             throw new ArgumentException($"Type {typeof(T)} is not a {typeof(DotView)}");
//         }
//         _dotPool.Enqueue(dot as DotView);
//         dot.gameObject.SetActive(false);
//     }

//     public override void Clear<T>(Queue<T> pool)
//     {
//         base.Clear(pool);
//     }
// }