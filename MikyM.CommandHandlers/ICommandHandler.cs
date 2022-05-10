using MikyM.CommandHandlers.Commands;
using MikyM.Common.Utilities.Results;

namespace MikyM.CommandHandlers;

/// <summary>
/// Command handler marker interface, used only internally
/// </summary>
public interface ICommandHandler
{
}

/// <summary>
/// Command handler
/// </summary>
/// <typeparam name="TCommand">Command type implementing <see cref="ICommand"/></typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : class, ICommand
{
    /// <summary>
    /// Handles the given command
    /// </summary>
    /// <param name="command">Command to handle</param>
    /// <returns>The <see cref="Result"/> of the operation </returns>
    Task<Result> HandleAsync(TCommand command);
}

/// <summary>
/// Command handler that returns a specific result
/// </summary>
/// <typeparam name="TCommand">Command type implementing <see cref="ICommand{TResult}"/></typeparam>
/// <typeparam name="TResult">Result of the <see cref="ICommand{TResult}"/></typeparam>
public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : class, ICommand<TResult>
{
    /// <summary>
    /// Handles the given command
    /// </summary>
    /// <param name="command">Command to handle</param>
    /// <returns>The <see cref="Result"/> of the operation containing a <typeparamref name="TResult"/> if any</returns>
    Task<Result<TResult>> HandleAsync(TCommand command);
}