using Autofac;
using Microsoft.Extensions.Options;
using MikyM.Autofac.Extensions;

namespace MikyM.CommandHandlers;

/// <summary>
/// Command handler options.
/// </summary>
[PublicAPI]
public sealed class CommandHandlerConfiguration : IOptions<CommandHandlerConfiguration>
{
    internal CommandHandlerConfiguration(ContainerBuilder builder)
    {
        Builder = builder;
    }

    internal ContainerBuilder Builder { get; set; }

    /// <summary>
    /// Gets or sets the default lifetime of command handlers.
    /// </summary>
    public Lifetime DefaultLifetime { get; set; } = Lifetime.InstancePerLifetimeScope;

    /// <inheritdoc/>
    public CommandHandlerConfiguration Value => this;
}
