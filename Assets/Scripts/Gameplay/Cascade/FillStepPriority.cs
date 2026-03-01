/// <summary>
/// Priority for ordering fill steps within a phase. Higher value runs first; ties broken by sequence.
/// </summary>
public enum FillStepPriority
{
    Low = 0,
    Normal = 100,
    High = 200,
    VeryHigh = 300
}
