using System;

public interface IClearable : IModel
{
    Func<bool> ShouldClear { get; set; }
    void Clear();
    
}