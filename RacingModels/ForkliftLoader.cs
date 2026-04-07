namespace RacingModels;

public sealed class ForkliftLoader : ILoader
{
    private readonly TimeSpan _recoveryDuration;

    public ForkliftLoader(TimeSpan recoveryDuration)
    {
        _recoveryDuration = recoveryDuration;
    }

    public string EquipmentName => "Погрузчик";

    public void Attach(Bolide bolide)
    {
        bolide.Collision += OnCollision;
    }

    private void OnCollision(Bolide sender, CollisionEventArgs e)
    {
        sender.EnterCollisionRecovery(_recoveryDuration);
    }
}
