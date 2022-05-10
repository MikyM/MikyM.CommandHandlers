using System.Text.Json;

namespace MikyM.CommandHandlers.Commands;

/// <summary>
/// Base command implementation
/// </summary>
public abstract class CommandBase : ICommand
{
    /// <summary>
    /// Serializes this to json using <see cref="JsonSerializer"/>
    /// </summary>
    public override string ToString()
        => JsonSerializer.Serialize(this);
}

/// <summary>
/// Base command with a result implementation
/// </summary>
/// <typeparam name="TResult">The type of the result of this command</typeparam>
public abstract class CommandBase<TResult> : ICommand<TResult>
{
    /// <summary>
    /// Serializes this to json using <see cref="JsonSerializer"/>
    /// </summary>
    public override string ToString()
        => JsonSerializer.Serialize(this);
}
