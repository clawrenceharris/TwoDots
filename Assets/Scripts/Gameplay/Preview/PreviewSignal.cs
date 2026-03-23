using System;

[Flags]
public enum PreviewSignal
{
    None = 0,
    OneHitLeft = 1 << 0,
    ConnectedActive = 1 << 1,
    Pulse = 1 << 2,
    Shake = 1 << 3,
    WingFlap = 1 << 4,
    GhostTargeting = 1 << 5,
    ValidTrigger = 1 << 6
}
