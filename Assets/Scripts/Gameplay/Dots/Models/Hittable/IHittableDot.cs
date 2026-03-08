public interface IHittableDot : IDotModel
{
    int HitCount { get; set; }
    int HitMax { get; set; }
    IClearableDot Clearable { get; set; }
    bool ShouldHit();
    void Hit();
}