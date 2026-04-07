using System.Reflection;

namespace RacingModels;

public static class BolideEventReflection
{
    public static IReadOnlyList<string> GetBolideEventNames()
    {
        return typeof(Bolide)
            .GetEvents(BindingFlags.Instance | BindingFlags.Public)
            .Select(e => $"{e.Name} ({DescribeHandlerType(e.EventHandlerType)})")
            .OrderBy(s => s)
            .ToArray();
    }

    private static string DescribeHandlerType(Type? handlerType)
    {
        if (handlerType is null)
            return "?";
        return handlerType.Name;
    }
}
