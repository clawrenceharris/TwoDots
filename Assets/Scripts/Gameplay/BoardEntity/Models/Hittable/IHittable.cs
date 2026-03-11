using System.Collections.Generic;

public interface IHittable
{
    List<HitConditionType> Conditions { get; set; }
    int HitCount { get; set; }
    int HitMax { get; set; }
    IClearable Clearable { get; set; }
    bool ShouldHit();
    void Hit();
}