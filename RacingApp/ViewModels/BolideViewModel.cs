using CommunityToolkit.Mvvm.ComponentModel;
using RacingModels;

namespace RacingApp.ViewModels;

public partial class BolideViewModel : ViewModelBase
{
    public const double TrackCanvasWidth = 1230;

    public const double TrackCanvasHeight = 580;
    public const double CarWidth = 52;
    public const double CarHeight = 16;
    public const double LaneHeight = 68;
    public const double TopOffset = 12;

    public BolideViewModel(Bolide model)
    {
        Model = model;
        model.PropertyChanged += (_, e) => SyncFromModel(e.PropertyName);
        SyncFromModel(null);
    }

    public Bolide Model { get; }

    [ObservableProperty] private double _canvasX;
    [ObservableProperty] private double _canvasY;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _status = string.Empty;

    public void SyncFromModel(string? propertyName)
    {
        if (propertyName is null ||
            propertyName == nameof(Bolide.TrackProgress) ||
            propertyName == nameof(Bolide.LaneIndex))
        {
            CanvasX = Model.TrackProgress * (TrackCanvasWidth - CarWidth);
            CanvasY = TopOffset + Model.LaneIndex * LaneHeight;
        }

        if (propertyName is null || propertyName == nameof(Bolide.StatusMessage))
            Status = Model.StatusMessage;

        if (propertyName is null || propertyName == nameof(Bolide.Name))
            Title = Model.Name;
    }
}
