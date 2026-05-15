using System.Security.Cryptography;

namespace DiceRoller.Services;

public sealed class CryptoRandomSource : IRandomSource
{
    /// <summary>
    /// Returns a cryptographically strong uniform integer in [minInclusive, maxInclusive].
    /// Delegates to RandomNumberGenerator.GetInt32 which uses rejection sampling
    /// internally to eliminate modulo bias.
    /// System.Random is deliberately NOT used (Constitution Principle II).
    /// </summary>
    public int Next(int minInclusive, int maxInclusive) =>
        RandomNumberGenerator.GetInt32(minInclusive, maxInclusive + 1);
}
