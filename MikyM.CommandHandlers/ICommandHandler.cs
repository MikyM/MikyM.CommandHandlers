using MikyM.Common.Utilities.Results;

namespace MikyM.CommandHandlers;

/// <summary>
/// Defines a base command handler. <b>Used ONLY as a marker interface.</b>
/// </summary>
[PublicAPI]
public interface ICommandHandler
{
}

/// <summary>
/// Defines a command handler without a concrete result.
/// </summary>
/// <typeparam name="TCommand">Command type implementing <see cref="ICommand"/>.</typeparam>
[PublicAPI]
public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : class, ICommand
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
public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : class, ICommand<TResult>
{
    /// <summary>
    /// Handles the given command,
    /// </summary>
    /// <param name="command">Command to handle,</param>
    /// <returns>The <see cref="Result"/> of the operation containing a <typeparamref name="TResult"/> if any,</returns>
    Task<Result<TResult>> HandleAsync(TCommand command);
}
