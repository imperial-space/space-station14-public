namespace Content.Shared.Imperial.SST.BootSweep;

[ByRefEvent]
public record struct BootSweepAttemptEvent(bool Cancelled);