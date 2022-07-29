using Autofac;

namespace MikyM.CommandHandlers;

/// <summary>
/// A factory for <see cref="ICommandHandlerBase"/>.
/// </summary>
[PublicAPI]
public interface ICommandHandlerFactory
{
    /// <summary>
    /// Gets an <see cref="ICommandHandlerBase"/> of a given type.
    /// </summary>
    /// <typeparam name="TCommandHandler">Type of the <see cref="ICommandHandlerBase"/> to get.</typeparam>
    /// <returns>Wanted <see cref="ICommandHandlerBase"/>.</returns>
    TCommandHandler GetHandler<TCommandHandler>() where TCommandHandler : class, ICommandHandlerBase;

    /// <summary>
    /// Gets an <see cref="ICommandHandlerBase"/> for a given <see cref="ICommand{TResult}"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the <see cref="ICommand{TResult}"/>.</typeparam>
    /// <typeparam name="TResult">Type of the command result.</typeparam>
    /// <returns>Wanted <see cref="ICommandHandlerBase"/>.</returns>
    ICommandHandler<TCommand, TResult> GetHandlerFor<TCommand, TResult>() where TCommand : class, ICommand<TResult>;
    /// <summary>
    /// Gets an <see cref="ICommandHandlerBase"/> for a given <see cref="ICommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">Type of the <see cref="ICommand"/>.</typeparam>
    /// <returns>Wanted <see cref="ICommandHandlerBase"/>.</returns>
    ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : class, ICommand;
}

/// <inheritdoc cref="ICommandHandlerFactory"/>
public class CommandHandlerFactory : ICommandHandlerFactory
{
    private readonly ILifetimeScope _lifetimeScope;

    /// <summary>
    /// Creates a new instance of <see cref="CommandHandlerFactory"/>.
    /// </summary>
    /// <param name="lifetimeScope">Autofac's <see cref="ILifetimeScope"/>.</param>
    public CommandHandlerFactory(ILifetimeScope lifetimeScope)
    {
        _lifetimeScope = lifetimeScope;
    }

    /// <inheritdoc />
    public TCommandHandler GetHandler<TCommandHandler>() where TCommandHandler : class, ICommandHandlerBase
    {
        if (!typeof(TCommandHandler).IsInterface)
            throw new ArgumentException("Due to Autofac limitations you must use interfaces");

        return _lifetimeScope.Resolve<TCommandHandler>();
    }

    /// <inheritdoc />
    public ICommandHandler<TCommand> GetHandlerFor<TCommand>() where TCommand : class, ICommand
        => _lifetimeScope.Resolve<ICommandHandler<TCommand>>();

    /// <inheritdoc />
    public ICommandHandler<TCommand ,TResult> GetHandlerFor<TCommand, TResult>() where TCommand : class, ICommand<TResult>
        => _lifetimeScope.Resolve<ICommandHandler<TCommand, TResult>>();
}
