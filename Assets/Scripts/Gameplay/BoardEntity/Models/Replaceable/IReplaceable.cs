public interface IReplaceable
{
    Dot NewDot { get; set; }
    void Replace();
    bool ShouldReplace();
}