namespace DiceRoller.Services;

public interface IRandomSource
{
    /// <summary>Returns a uniform random integer in [minInclusive, maxInclusive].</summary>
    int Next(int minInclusive, int maxInclusive);
}
