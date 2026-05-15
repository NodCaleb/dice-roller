namespace DiceRoller.Models;

public record ParseResult<T>
{
    public bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public string? ErrorMessage { get; init; }

    public static ParseResult<T> Ok(T value) =>
        new() { IsSuccess = true, Value = value };

    public static ParseResult<T> Fail(string message) =>
        new() { IsSuccess = false, ErrorMessage = message };
}
