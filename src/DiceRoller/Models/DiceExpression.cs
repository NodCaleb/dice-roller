namespace DiceRoller.Models;

public record DiceExpression(int Count, DieType Die, int Modifier)
{
    /// <summary>
    /// Returns the canonical string form:
    ///   - Count always explicit (d20 → "1d20")
    ///   - Die separator always lowercase 'd'
    ///   - Zero modifier omitted ("1d20+0" → "1d20")
    ///   - Non-zero modifier with explicit sign ("3d8+2", "2d6-1")
    /// </summary>
    public string ToCanonical()
    {
        var base_ = $"{Count}d{(int)Die}";
        return Modifier == 0 ? base_ : $"{base_}{Modifier:+0;-0}";
    }

    public override string ToString() => ToCanonical();
}
