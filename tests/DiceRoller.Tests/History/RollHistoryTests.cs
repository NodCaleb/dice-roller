using DiceRoller.Models;
using DiceRoller.Services;

namespace DiceRoller.Tests.History;

public class RollHistoryTests
{
    private readonly RollHistory _sut = new();

    private static DiceExpression Expr(int count, DieType die, int mod = 0) =>
        new(count, die, mod);

    [Fact]
    public void NewHistory_HasEmptyEntries()
    {
        Assert.Empty(_sut.Entries);
    }

    [Fact]
    public void Add_FirstEntry_IsAtIndex0()
    {
        var expr = Expr(1, DieType.D20);
        _sut.Add(expr);
        Assert.Single(_sut.Entries);
        Assert.Equal(expr, _sut.Entries[0]);
    }

    [Fact]
    public void Add_SecondDistinctEntry_IsAtIndex0_FirstAtIndex1()
    {
        var first = Expr(1, DieType.D20);
        var second = Expr(2, DieType.D6);
        _sut.Add(first);
        _sut.Add(second);
        Assert.Equal(second, _sut.Entries[0]);
        Assert.Equal(first, _sut.Entries[1]);
    }

    [Fact]
    public void Add_SameCanonicalExpression_MovesToTop_CountUnchanged()
    {
        var first = Expr(1, DieType.D20);
        var second = Expr(2, DieType.D6);
        _sut.Add(first);
        _sut.Add(second);
        _sut.Add(first); // repeat first
        Assert.Equal(2, _sut.Entries.Count);
        Assert.Equal(first, _sut.Entries[0]);
        Assert.Equal(second, _sut.Entries[1]);
    }

    [Fact]
    public void Add_7DistinctEntries_CountIs7()
    {
        foreach (var die in DieTypeExtensions.AllValues)
            _sut.Add(Expr(1, die));
        Assert.Equal(7, _sut.Entries.Count);
    }

    [Fact]
    public void Add_8thDistinctEntry_DropsOldestEntry_CountStaysAt7()
    {
        foreach (var die in DieTypeExtensions.AllValues)
            _sut.Add(Expr(1, die));

        var eighth = Expr(2, DieType.D4);
        _sut.Add(eighth);

        Assert.Equal(7, _sut.Entries.Count);
        Assert.Equal(eighth, _sut.Entries[0]);
    }

    [Fact]
    public void Add_8thDistinctEntry_OldestIsEvicted()
    {
        // The first added (D4) becomes the oldest after 7 adds
        var oldest = Expr(1, DieType.D4);
        _sut.Add(oldest);
        foreach (var die in DieTypeExtensions.AllValues.Where(d => d != DieType.D4))
            _sut.Add(Expr(1, die));

        var newEntry = Expr(2, DieType.D4); // different canonical than oldest
        _sut.Add(newEntry);

        Assert.DoesNotContain(_sut.Entries, e => e == oldest);
    }

    [Fact]
    public void Add_RepeatOf7thEntry_MovesToTop_CountStaysAt7()
    {
        foreach (var die in DieTypeExtensions.AllValues)
            _sut.Add(Expr(1, die));

        var oldest = _sut.Entries[^1];
        _sut.Add(oldest); // move oldest to top

        Assert.Equal(7, _sut.Entries.Count);
        Assert.Equal(oldest, _sut.Entries[0]);
    }

    [Fact]
    public void Entries_AreOrderedMostRecentFirst()
    {
        var a = Expr(1, DieType.D4);
        var b = Expr(1, DieType.D6);
        var c = Expr(1, DieType.D8);
        _sut.Add(a);
        _sut.Add(b);
        _sut.Add(c);
        Assert.Equal(new[] { c, b, a }, _sut.Entries);
    }

    [Fact]
    public void Add_CanonicalEquivalent_DeduplicatesCorrectly()
    {
        // d20 and 1d20 have same canonical form "1d20"
        var expr1 = Expr(1, DieType.D20, 0);
        var expr2 = Expr(1, DieType.D20, 0); // structurally identical, same canonical
        _sut.Add(expr1);
        _sut.Add(expr2);
        Assert.Single(_sut.Entries);
    }
}
