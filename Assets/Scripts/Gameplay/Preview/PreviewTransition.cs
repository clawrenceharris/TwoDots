public readonly struct PreviewTransition
{
    public string EntityId { get; }
    public PreviewSignal Signal { get; }
    public PreviewTransitionType Type { get; }

    public PreviewTransition(string entityId, PreviewSignal signal, PreviewTransitionType type)
    {
        EntityId = entityId;
        Signal = signal;
        Type = type;
    }
}
