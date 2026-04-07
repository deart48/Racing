using RacingModels;

namespace RacingApp;

internal static class SimulationDefaults
{
    public static TimeSpan TireChangeDuration => TimeSpan.FromSeconds(2.5);
    public static TimeSpan LoaderRecoveryDuration => TimeSpan.FromSeconds(4.0);

    public static double EngineTickSeconds => 0.05;

    public static BolidePhysicsParameters CreateStandardPhysics(double tickSeconds) =>
        new(
            progressPerSecond: 0.09,
            tireWearPerSecond: 1.15,
            collisionChancePerTick: 0.0015,
            voluntaryPitChancePerTickWhenUnder20Percent: 0.008,
            tickSeconds: tickSeconds,
            mandatoryPitAtOrBelowTreadPercent: 1.0,
            voluntaryPitBandMaxTreadPercent: 20.0);
}
