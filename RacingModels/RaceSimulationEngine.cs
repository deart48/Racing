namespace RacingModels;

public sealed class RaceSimulationEngine : IDisposable
{
    private readonly object _gate = new();
    private readonly List<Bolide> _bolides = new();
    private readonly Random _random = new();
    private CancellationTokenSource? _cts;
    private Task? _loopTask;

    public IReadOnlyList<Bolide> Bolides
    {
        get
        {
            lock (_gate)
                return _bolides.ToArray();
        }
    }

    public void AddBolide(Bolide bolide)
    {
        lock (_gate)
            _bolides.Add(bolide);
    }

    public bool RemoveBolide(Bolide bolide)
    {
        lock (_gate)
            return _bolides.Remove(bolide);
    }

    public void Start(TimeSpan tickInterval, Action onTickUiMarshal)
    {
        Stop();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        _loopTask = Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(tickInterval, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                Bolide[] snapshot;
                lock (_gate)
                    snapshot = _bolides.ToArray();

                foreach (var b in snapshot)
                    b.Tick(_random);

                onTickUiMarshal();
            }
        }, token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        try
        {
            _loopTask?.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException)
        {
        }
        _cts?.Dispose();
        _cts = null;
        _loopTask = null;
    }

    public void Dispose()
    {
        Stop();
    }
}
