namespace RacingModels;

public sealed class BolidePhysicsParameters
{
    public BolidePhysicsParameters(
        double progressPerSecond,
        double tireWearPerSecond,
        double collisionChancePerTick,
        double voluntaryPitChancePerTickWhenUnder20Percent,
        double tickSeconds,
        double mandatoryPitAtOrBelowTreadPercent = 1.0,
        double voluntaryPitBandMaxTreadPercent = 20.0)
    {
        ProgressPerSecond = progressPerSecond;
        TireWearPerSecond = tireWearPerSecond;
        CollisionChancePerTick = collisionChancePerTick;
        VoluntaryPitChancePerTickWhenUnder20Percent = voluntaryPitChancePerTickWhenUnder20Percent;
        TickSeconds = tickSeconds;
        MandatoryPitAtOrBelowTreadPercent = mandatoryPitAtOrBelowTreadPercent;
        VoluntaryPitBandMaxTreadPercent = voluntaryPitBandMaxTreadPercent;
    }

    public double ProgressPerSecond { get; }
    public double TireWearPerSecond { get; }
    public double CollisionChancePerTick { get; }
    public double VoluntaryPitChancePerTickWhenUnder20Percent { get; }
    public double TickSeconds { get; }
    public double MandatoryPitAtOrBelowTreadPercent { get; }
    public double VoluntaryPitBandMaxTreadPercent { get; }
}
