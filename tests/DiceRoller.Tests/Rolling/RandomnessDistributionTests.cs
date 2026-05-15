using DiceRoller.Models;
using DiceRoller.Services;

namespace DiceRoller.Tests.Rolling;

public class RandomnessDistributionTests
{
    private static readonly DiceRollerService _service = new(new CryptoRandomSource());

    // Chi-square critical values at alpha = 0.001 (0.1% significance level).
    // Using a very conservative threshold to minimise flakiness while still
    // catching genuinely non-uniform distributions.
    private static readonly Dictionary<int, double> ChiSquareCriticals = new()
    {
        [3]  = 16.27,  // D4
        [5]  = 20.52,  // D6
        [7]  = 24.32,  // D8
        [9]  = 27.88,  // D10
        [11] = 31.26,  // D12
        [19] = 43.82,  // D20
        [99] = 148.23, // D100
    };

    [Theory]
    [InlineData(DieType.D4,   1_000)]
    [InlineData(DieType.D6,   1_000)]
    [InlineData(DieType.D8,   1_000)]
    [InlineData(DieType.D10,  1_000)]
    [InlineData(DieType.D12,  1_000)]
    [InlineData(DieType.D20,  2_000)]
    [InlineData(DieType.D100, 10_000)]
    public void Roll_Distribution_IsStatisticallyUniform(DieType dieType, int numberOfRolls)
    {
        var faces = (int)dieType;
        var expr = new DiceExpression(1, dieType, 0);
        var counts = new int[faces + 1]; // index 1..faces, index 0 unused

        for (int i = 0; i < numberOfRolls; i++)
        {
            var result = _service.Roll(expr);
            counts[result.DiceValues[0]]++;
        }

        double expected = (double)numberOfRolls / faces;
        double chiSquare = 0;
        for (int face = 1; face <= faces; face++)
        {
            double diff = counts[face] - expected;
            chiSquare += (diff * diff) / expected;
        }

        double critical = ChiSquareCriticals[faces - 1];
        Assert.True(
            chiSquare < critical,
            $"Chi-square statistic {chiSquare:F2} exceeds critical value {critical:F2} " +
            $"for {dieType} over {numberOfRolls} rolls — distribution appears non-uniform.");
    }
}
