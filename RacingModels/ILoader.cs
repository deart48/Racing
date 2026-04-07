namespace RacingModels;

public interface ILoader
{
    string EquipmentName { get; }

    void Attach(Bolide bolide);
}
