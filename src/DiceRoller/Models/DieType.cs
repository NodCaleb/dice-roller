namespace DiceRoller.Models;

public enum DieType
{
    D4 = 4,
    D6 = 6,
    D8 = 8,
    D10 = 10,
    D12 = 12,
    D20 = 20,
    D100 = 100,
}

public static class DieTypeExtensions
{
    public static readonly IReadOnlyList<DieType> AllValues =
    [
        DieType.D4, DieType.D6, DieType.D8, DieType.D10,
        DieType.D12, DieType.D20, DieType.D100,
    ];

    public static readonly IReadOnlySet<int> ValidSides =
        new HashSet<int>(AllValues.Select(d => (int)d));

    public static string ToLabel(this DieType die) => $"d{(int)die}";
}
