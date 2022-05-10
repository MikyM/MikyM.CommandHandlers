using Autofac;
using Microsoft.Extensions.Options;
using MikyM.Autofac.Extensions;

namespace MikyM.CommandHandlers.Helpers;

/// <summary>
/// Command handler options
/// </summary>
public sealed class CommandHandlerConfiguration : IOptions<CommandHandlerConfiguration>
{
    internal CommandHandlerConfiguration(ContainerBuilder builder)
    {
        Builder = builder;
    }

    internal ContainerBuilder Builder { get; set; }

    /// <summary>
    /// Gets or sets the default lifetime for base generic data services
    /// </summary>
    public Lifetime DefaultLifetime { get; set; } = Lifetime.InstancePerLifetimeScope;

    /// <inheritdoc />
    public CommandHandlerConfiguration Value => this;
}