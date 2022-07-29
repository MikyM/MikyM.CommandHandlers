using MikyM.Common.Utilities.Results;

namespace MikyM.CommandHandlers;

/// <summary>
/// Defines a base command handler. <b>Shouldn't be implemented manually, implement <see cref="ICommandHandler{TCommand}"/> or <see cref="ICommandHandler{TCommand,TResult}"/> instead.</b>
/// </summary>
[PublicAPI]
public interface ICommandHandlerBase
{
}

/// <summary>
/// Defines a command handler without a concrete result.
/// </summary>
/// <typeparam name="TCommand">Command type implementing <see cref="ICommand"/>.</typeparam>
[PublicAPI]
public interface ICommandHandler<in TCommand> : ICommandHandlerBase where TCommand : class, ICommand
{
    /// <summary>
    /// Handles the given command.
    /// </summary>
    /// <param name="command">Command to handle.</param>
    /// <returns>The <see cref="Result"/> of the operation.</returns>
    Task<Result> HandleAsync(TCommand command);
}

/// <summary>
/// Defines a command handler with a concrete result.
/// </summary>
/// <typeparam name="TCommand">Command type implementing <see cref="ICommand{TResult}"/>,</typeparam>
/// <typeparam name="TResult">Result of the <see cref="ICommand{TResult}"/>,</typeparam>
[PublicAPI]
public interface ICommandHandler<in TCommand, TResult> : ICommandHandlerBase where TCommand : class, ICommand<TResult>
{
    /// <summary>
    /// Handles the given command,
    /// </summary>
    /// <param name="command">Command to handle,</param>
    /// <returns>The <see cref="Result"/> of the operation containing a <typeparamref name="TResult"/> if any,</returns>
    Task<Result<TResult>> HandleAsync(TCommand command);
}
