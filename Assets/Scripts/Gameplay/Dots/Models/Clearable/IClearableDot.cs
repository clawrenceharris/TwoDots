using System;

public interface IClearableDot : IDotModel
{
    Func<bool> ShouldClear { get; set; }
    void Clear();
    
}