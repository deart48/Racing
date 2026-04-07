namespace RacingModels;

public sealed class Mechanic
{
    private readonly TimeSpan _tireChangeDuration;

    public Mechanic(TimeSpan tireChangeDuration)
    {
        _tireChangeDuration = tireChangeDuration;
    }

    public void Attach(Bolide bolide)
    {
        bolide.TiresWorn += OnTiresWorn;
    }

    private void OnTiresWorn(Bolide sender, TireWearEventArgs e)
    {
        sender.EnterPitStop(_tireChangeDuration);
    }
}
