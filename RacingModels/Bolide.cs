using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RacingModels;

public sealed class Bolide : INotifyPropertyChanged
{
    private double _trackProgress;
    private double _tireHealth;
    private BolideState _state;
    private double _serviceTimeRemainingSeconds;
    private string _statusMessage = string.Empty;

    public Bolide(string name, int laneIndex, BolidePhysicsParameters physics)
    {
        Name = name;
        LaneIndex = laneIndex;
        Physics = physics;
        _tireHealth = 100;
        _trackProgress = 0;
        _state = BolideState.Racing;
        UpdateStatusMessage();
    }

    public string Name { get; }
    public int LaneIndex { get; }
    public BolidePhysicsParameters Physics { get; }

    public double TrackProgress
    {
        get => _trackProgress;
        private set => SetField(ref _trackProgress, value);
    }

    public double TireHealth
    {
        get => _tireHealth;
        private set => SetField(ref _tireHealth, Math.Clamp(value, 0, 100));
    }

    public BolideState State
    {
        get => _state;
        private set => SetField(ref _state, value);
    }

    public double ServiceTimeRemainingSeconds
    {
        get => _serviceTimeRemainingSeconds;
        private set => SetField(ref _serviceTimeRemainingSeconds, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set => SetField(ref _statusMessage, value);
    }


    public event BolideTireWearHandler? TiresWorn;

    public event BolideCollisionHandler? Collision;

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Tick(Random random)
    {
        var dt = Physics.TickSeconds;

        switch (State)
        {
            case BolideState.Racing:
                TrackProgress += Physics.ProgressPerSecond * dt;
                if (TrackProgress >= 1.0)
                    TrackProgress -= 1.0;

                TireHealth -= Physics.TireWearPerSecond * dt;

                var mandatoryBelow = Physics.MandatoryPitAtOrBelowTreadPercent;
                var voluntaryMax = Physics.VoluntaryPitBandMaxTreadPercent;

                if (TireHealth <= mandatoryBelow)
                {
                    var cause = TireHealth <= 0 ? TirePitCause.Depleted : TirePitCause.MandatoryLowTread;
                    TiresWorn?.Invoke(this, new TireWearEventArgs(this, TireHealth, cause));
                }
                else if (TireHealth < voluntaryMax
                         && TireHealth > mandatoryBelow
                         && random.NextDouble() < Physics.VoluntaryPitChancePerTickWhenUnder20Percent)
                {
                    TiresWorn?.Invoke(this, new TireWearEventArgs(this, TireHealth, TirePitCause.VoluntaryUnder20Percent));
                }
                else if (TireHealth > mandatoryBelow
                         && random.NextDouble() < Physics.CollisionChancePerTick)
                {
                    Collision?.Invoke(this, new CollisionEventArgs(this, Physics.CollisionChancePerTick));
                }

                break;

            case BolideState.PitStop:
            case BolideState.CollisionRecovery:
                ServiceTimeRemainingSeconds -= dt;
                if (ServiceTimeRemainingSeconds <= 0)
                {
                    ServiceTimeRemainingSeconds = 0;
                    if (State == BolideState.PitStop)
                        TireHealth = 100;
                    State = BolideState.Racing;
                }

                break;
        }

        UpdateStatusMessage();
    }

    public void EnterPitStop(TimeSpan serviceDuration)
    {
        if (State != BolideState.Racing)
            return;
        State = BolideState.PitStop;
        ServiceTimeRemainingSeconds = serviceDuration.TotalSeconds;
        UpdateStatusMessage();
    }

    public void EnterCollisionRecovery(TimeSpan recoveryDuration)
    {
        if (State != BolideState.Racing)
            return;
        State = BolideState.CollisionRecovery;
        ServiceTimeRemainingSeconds = recoveryDuration.TotalSeconds;
        UpdateStatusMessage();
    }

    private void UpdateStatusMessage()
    {
        StatusMessage = State switch
        {
            BolideState.Racing => $"Гонка • шины {TireHealth:0}%",
            BolideState.PitStop => $"Боксы • {ServiceTimeRemainingSeconds:0.0} с",
            BolideState.CollisionRecovery => $"Погрузчик • {ServiceTimeRemainingSeconds:0.0} с",
            _ => string.Empty,
        };
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value))
            return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
