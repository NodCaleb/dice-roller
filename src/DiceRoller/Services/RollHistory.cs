using DiceRoller.Models;

namespace DiceRoller.Services;

public sealed class RollHistory : IRollHistory
{
    private const int MaxEntries = 7;
    private readonly List<DiceExpression> _entries = new();

    public IReadOnlyList<DiceExpression> Entries => _entries;

    public void Add(DiceExpression expression)
    {
        var canonical = expression.ToCanonical();

        // Remove existing entry with the same canonical key (deduplication)
        var existing = _entries.FindIndex(e => e.ToCanonical() == canonical);
        if (existing >= 0)
            _entries.RemoveAt(existing);

        // Insert at top (most recent)
        _entries.Insert(0, expression);

        // Trim to max capacity
        if (_entries.Count > MaxEntries)
            _entries.RemoveAt(_entries.Count - 1);
    }
}
