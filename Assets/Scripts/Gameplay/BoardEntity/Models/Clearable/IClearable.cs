using System;

public interface IClearable : IModel
{
    /// <summary>
    /// A rule that determines if the clearable model should be cleared.
    /// </summary>
    IRule ClearRule { get; set; }

    /// <summary>
    /// Checks if the clearable model should be cleared.
    /// </summary>
    /// <returns>True if the clearable model should be cleared, false otherwise</returns>
    bool ShouldClear();

    /// <summary>
    /// Clears the clearable model by executing any needed clear logic.
    /// </summary>
    void Clear();
    
}