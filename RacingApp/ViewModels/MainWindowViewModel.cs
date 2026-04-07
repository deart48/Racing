using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RacingModels;

namespace RacingApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private const int MaxLanes = 8;
    private readonly RaceSimulationEngine _engine = new();
    private readonly Mechanic _mechanic;
    private readonly ILoader _loader;
    private bool _isRunning;

    public MainWindowViewModel()
    {
        _mechanic = new Mechanic(SimulationDefaults.TireChangeDuration);
        _loader = new ForkliftLoader(SimulationDefaults.LoaderRecoveryDuration);
    }

    public ObservableCollection<BolideViewModel> Bolides { get; } = new();
    public ObservableCollection<string> EventLog { get; } = new();

    [ObservableProperty] private string _newBolideName = string.Empty;

    [ObservableProperty] private string _simulationStatus = "Симуляция остановлена";

    public bool IsRunning
    {
        get => _isRunning;
        private set => SetProperty(ref _isRunning, value);
    }

    [RelayCommand]
    private void AddBolide()
    {
        var name = string.IsNullOrWhiteSpace(NewBolideName)
            ? $"Болид {Bolides.Count + 1}"
            : NewBolideName.Trim();

        if (Bolides.Any(b => string.Equals(b.Model.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            SimulationStatus = "Болид с таким именем уже есть. Введите другое имя.";
            return;
        }

        var lane = Bolides.Count % MaxLanes;
        var tick = SimulationDefaults.EngineTickSeconds;
        var physics = SimulationDefaults.CreateStandardPhysics(tick);
        var bolide = new Bolide(name, lane, physics);

        _mechanic.Attach(bolide);
        _loader.Attach(bolide);

        bolide.TiresWorn += (_, e) =>
            PostLog(FormatTiresPitMessage(e));

        bolide.Collision += (_, e) =>
            PostLog($"Столкновение → вызван погрузчик для {e.Bolide.Name} (вероятность за тик {e.ReportedProbability:P3}).");

        _engine.AddBolide(bolide);
        Bolides.Add(new BolideViewModel(bolide));
    }

    [RelayCommand]
    private void ToggleSimulation()
    {
        if (IsRunning)
        {
            _engine.Stop();
            IsRunning = false;
            SimulationStatus = "Симуляция остановлена";
            return;
        }

        if (Bolides.Count == 0)
        {
            SimulationStatus = "Добавьте хотя бы один болид.";
            return;
        }

        var tick = TimeSpan.FromSeconds(SimulationDefaults.EngineTickSeconds);
        RunSafely(() =>
        {
            _engine.Start(tick, () => RunOnUiThread(RefreshAllVisuals));
            IsRunning = true;
            SimulationStatus = "Симуляция запущена (фоновый поток)";
        }, err => SimulationStatus = $"Ошибка запуска: {err}");
    }

    [RelayCommand]
    private void ClearLog()
    {
        EventLog.Clear();
    }

    private void RefreshAllVisuals()
    {
        foreach (var vm in Bolides)
            vm.SyncFromModel(null);
    }

    private static string FormatTiresPitMessage(TireWearEventArgs e)
    {
        var name = e.Bolide.Name;
        var pct = e.RemainingTirePercent;
        return e.Cause switch
        {
            TirePitCause.Depleted =>
                $"Шины износились полностью → {name} заезжает в боксы.",
            TirePitCause.MandatoryLowTread =>
                $"Остаток шин ≤ 1% → обязательный бокс для {name} ({pct:0.#}%).",
            TirePitCause.VoluntaryUnder20Percent =>
                $"Низкий запас шин (< 20%) → {name} заезжает в боксы по решению экипажа ({pct:0.#}%).",
            _ => $"{name} заезжает в боксы (шины {pct:0.#}%).",
        };
    }

    private void PostLog(string message)
    {
        var line = $"{DateTime.Now:HH:mm:ss} — {message}";
        RunOnUiThread(() =>
        {
            EventLog.Insert(0, line);
            while (EventLog.Count > 80)
                EventLog.RemoveAt(EventLog.Count - 1);
        });
    }

    public void Dispose()
    {
        _engine.Dispose();
        GC.SuppressFinalize(this);
    }
}
