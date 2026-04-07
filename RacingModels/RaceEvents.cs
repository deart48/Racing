namespace RacingModels;


public enum TirePitCause
{

    Depleted,

    MandatoryLowTread,

    VoluntaryUnder20Percent,
}

public sealed class TireWearEventArgs : EventArgs
{
    public TireWearEventArgs(Bolide bolide, double remainingTirePercent, TirePitCause cause)
    {
        Bolide = bolide;
        RemainingTirePercent = remainingTirePercent;
        Cause = cause;
    }

    public Bolide Bolide { get; }
    public double RemainingTirePercent { get; }
    public TirePitCause Cause { get; }
}

public sealed class CollisionEventArgs : EventArgs
{
    public CollisionEventArgs(Bolide bolide, double reportedProbability)
    {
        Bolide = bolide;
        ReportedProbability = reportedProbability;
    }

    public Bolide Bolide { get; }
    public double ReportedProbability { get; }
}

public delegate void BolideTireWearHandler(Bolide sender, TireWearEventArgs e);

public delegate void BolideCollisionHandler(Bolide sender, CollisionEventArgs e);
