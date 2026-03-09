using System;

public interface IClearableDot : IModel
{
    Func<bool> ShouldClear { get; set; }
    void Clear();
    
}