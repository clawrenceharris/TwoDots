using System.Collections.Generic;

/// <summary>
/// A model associated with a board entity that can be hit.
/// </summary>
public interface IHittable
{
    /// <summary>The number of times the hittable has been hit.</summary>
    int HitCount { get; set; }
    /// <summary>The maximum number of times the hittable can be hit.</summary>
    int HitMax { get; set; }
    /// <summary>A rule that determines if the hittable model should be hit.</summary>
    IRule HitRule { get; }
    /// <summary>
    /// Checks if the hittable model should be hit.
    /// </summary>
    /// <returns>True if the hittable model should be hit, false otherwise</returns>
    bool ShouldHit();
    /// <summary>
    /// Hits the hittable model by incrementing the hit count
    /// </summary>
    
  
    void Hit();
    /// <summary>
    /// Checks if the hittable model should be cleared after the next hit.
    /// </summary>
    /// <returns>True if the hittable model should be cleared after the next hit, false otherwise</returns>
    bool ShouldClearAfterHit();


   
}