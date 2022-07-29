using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Features.Decorators;
using Microsoft.Extensions.Options;
using MikyM.Autofac.Extensions;
using MikyM.Autofac.Extensions.Attributes;
using MikyM.Autofac.Extensions.Extensions;
using MikyM.Common.Utilities.Extensions;

namespace MikyM.CommandHandlers;

/// <summary>
/// DI extensions for <see cref="ContainerBuilder"/>.
/// </summary>
[PublicAPI]
public static class DependancyInjectionExtensions
{
    /// <summary>
    /// Registers command handlers with the <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="builder">Current instance of <see cref="ContainerBuilder"/>.</param>
    /// <param name="configuration">Optional <see cref="CommandHandlerConfiguration"/> configuration.</param>
    /// <returns>Current <see cref="ContainerBuilder"/> instance.</returns>
    public static ContainerBuilder AddCommandHandlers(this ContainerBuilder builder, Action<CommandHandlerConfiguration>? configuration = null)
    {
        var config = new CommandHandlerConfiguration(builder);
        configuration?.Invoke(config);

        builder.Register(x => config).As<IOptions<CommandHandlerConfiguration>>().SingleInstance();

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var commandSet = assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(ICommandHandler<>)) && x.IsClass && !x.IsAbstract)
                .ToList();

            var commandResultSet = assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.IsGenericType && y.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)) && x.IsClass && !x.IsAbstract)
                .ToList();

            var commandSubSet = commandSet
                .Where(x => x.GetCustomAttribute<LifetimeAttribute>(false) is not null ||
                            x.GetCustomAttributes<InterceptedByAttribute>(false).Count() != 0 && x.IsClass &&
                            !x.IsAbstract)
                .ToList();

            var commandResultSubSet = commandResultSet
                .Where(x => x.GetCustomAttribute<LifetimeAttribute>(false) is not null ||
                            x.GetCustomAttributes<InterceptedByAttribute>(false).Count() != 0 && x.IsClass &&
                            !x.IsAbstract)
                .ToList();

            foreach (var type in commandSubSet)
            {
                var lifeAttr = type.GetCustomAttribute<LifetimeAttribute>(false);
                var intrAttrs = type.GetCustomAttributes<InterceptedByAttribute>(false);

                var registrationBuilder = builder.RegisterTypes(type).AsClosedInterfacesOf(typeof(ICommandHandler<>));

                var scope = lifeAttr?.Scope ?? config.DefaultLifetime;

                switch (scope)
                {
                    case Lifetime.SingleInstance:
                        registrationBuilder = registrationBuilder.SingleInstance();
                        break;
                    case Lifetime.InstancePerRequest:
                        registrationBuilder = registrationBuilder.InstancePerRequest();
                        break;
                    case Lifetime.InstancePerLifetimeScope:
                        registrationBuilder = registrationBuilder.InstancePerLifetimeScope();
                        break;
                    case Lifetime.InstancePerDependancy:
                        registrationBuilder = registrationBuilder.InstancePerDependency();
                        break;
                    case Lifetime.InstancePerMatchingLifetimeScope:
                        registrationBuilder =
                            registrationBuilder.InstancePerMatchingLifetimeScope(lifeAttr?.Tags.ToArray() ?? throw new InvalidOperationException());
                        break;
                    case Lifetime.InstancePerOwned:
                        if (lifeAttr?.Owned is null) throw new InvalidOperationException("Owned type was null");

                        registrationBuilder = registrationBuilder.InstancePerOwned(lifeAttr.Owned);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                var intrAttr = type.GetCustomAttribute<EnableInterceptionAttribute>(false);
                if (intrAttr is null) continue;

                bool interfaceEnabled = false;
                //bool classEnabled = false;
                foreach (var attr in intrAttrs)
                {
                    if (!interfaceEnabled)
                    {
                        registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
                        interfaceEnabled = true;
                    }

                    /*
                    switch (attr.Intercept)
                    {
                        case Intercept.Interfaces:
                            if (!interfaceEnabled)
                            {

                                registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
                                interfaceEnabled = true;
                            }
                            break;
                        case Intercept.Classes:
                            if (!classEnabled)
                            {

                                registrationBuilder = registrationBuilder.EnableClassInterceptors();
                                classEnabled = true;
                            }
                            break;
                        case Intercept.InterfacesAndClasses:
                            if (!classEnabled)
                            {

                                registrationBuilder = registrationBuilder.EnableClassInterceptors();
                                classEnabled = true;
                            }
                            if (!interfaceEnabled)
                            {

                                registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
                                interfaceEnabled = true;
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }*/

                    registrationBuilder = attr.IsAsync
                        ? registrationBuilder.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(attr.Interceptor))
                        : registrationBuilder.InterceptedBy(attr.Interceptor);
                }
            }

            foreach (var type in commandResultSubSet)
            {
                var lifeAttr = type.GetCustomAttribute<LifetimeAttribute>(false);
                var intrAttrs = type.GetCustomAttributes<InterceptedByAttribute>(false);

                var registrationBuilder = builder.RegisterTypes(type).AsClosedInterfacesOf(typeof(ICommandHandler<,>));

                var scope = lifeAttr?.Scope ?? config.DefaultLifetime;

                switch (scope)
                {
                    case Lifetime.SingleInstance:
                        registrationBuilder = registrationBuilder.SingleInstance();
                        break;
                    case Lifetime.InstancePerRequest:
                        registrationBuilder = registrationBuilder.InstancePerRequest();
                        break;
                    case Lifetime.InstancePerLifetimeScope:
                        registrationBuilder = registrationBuilder.InstancePerLifetimeScope();
                        break;
                    case Lifetime.InstancePerDependancy:
                        registrationBuilder = registrationBuilder.InstancePerDependency();
                        break;
                    case Lifetime.InstancePerMatchingLifetimeScope:
                        registrationBuilder =
                            registrationBuilder.InstancePerMatchingLifetimeScope(lifeAttr?.Tags.ToArray() ?? throw new InvalidOperationException());
                        break;
                    case Lifetime.InstancePerOwned:
                        if (lifeAttr?.Owned is null) throw new InvalidOperationException("Owned type was null");

                        registrationBuilder = registrationBuilder.InstancePerOwned(lifeAttr.Owned);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var intrAttr = type.GetCustomAttribute<EnableInterceptionAttribute>(false);
                if (intrAttr is null) continue;

                bool interfaceEnabled = false;
                //bool classEnabled = false;
                foreach (var attr in intrAttrs)
                {
                    if (!interfaceEnabled)
                    {
                        registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
                        interfaceEnabled = true;
                    }

                    /*switch (attr.Intercept)
                        {
                            case Intercept.Interfaces:
                                if (!interfaceEnabled)
                                {

                                    registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
                                    interfaceEnabled = true;
                                }
                                break;
                            case Intercept.Classes:
                                if (!classEnabled)
                                {

                                    registrationBuilder = registrationBuilder.EnableClassInterceptors();
                                    classEnabled = true;
                                }
                                break;
                            case Intercept.InterfacesAndClasses:
                                if (!classEnabled)
                                {

                                    registrationBuilder = registrationBuilder.EnableClassInterceptors();
                                    classEnabled = true;
                                }
                                if (!interfaceEnabled)
                                {

                                    registrationBuilder = registrationBuilder.EnableInterfaceInterceptors();
                                    interfaceEnabled = true;
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }*/

                    registrationBuilder = attr.IsAsync
                        ? registrationBuilder.InterceptedBy(
                            typeof(AsyncInterceptorAdapter<>).MakeGenericType(attr.Interceptor))
                        : registrationBuilder.InterceptedBy(attr.Interceptor);
                }
            }

            commandSet.RemoveAll(x => commandSubSet.Any(y => y == x));
            commandResultSet.RemoveAll(x => commandResultSubSet.Any(y => y == x));

            if (commandSet.Any())
            {
                switch (config.DefaultLifetime)
                {
                    case Lifetime.SingleInstance:
                        builder.RegisterTypes(commandSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<>))
                            .SingleInstance();
                        break;
                    case Lifetime.InstancePerRequest:
                        builder.RegisterTypes(commandSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<>))
                            .InstancePerRequest();
                        break;
                    case Lifetime.InstancePerLifetimeScope:
                        builder.RegisterTypes(commandSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<>))
                            .InstancePerLifetimeScope();
                        break;
                    case Lifetime.InstancePerMatchingLifetimeScope:
                        throw new NotSupportedException();
                    case Lifetime.InstancePerDependancy:
                        builder.RegisterTypes(commandSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<>))
                            .InstancePerDependency();
                        break;
                    case Lifetime.InstancePerOwned:
                        throw new NotSupportedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (commandResultSet.Any())
            {
                switch (config.DefaultLifetime)
                {
                    case Lifetime.SingleInstance:
                        builder.RegisterTypes(commandResultSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<,>))
                            .SingleInstance();
                        break;
                    case Lifetime.InstancePerRequest:
                        builder.RegisterTypes(commandResultSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<,>))
                            .InstancePerRequest();
                        break;
                    case Lifetime.InstancePerLifetimeScope:
                        builder.RegisterTypes(commandResultSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<,>))
                            .InstancePerLifetimeScope();
                        break;
                    case Lifetime.InstancePerMatchingLifetimeScope:
                        throw new NotSupportedException();
                    case Lifetime.InstancePerDependancy:
                        builder.RegisterTypes(commandResultSet.ToArray())
                            .AsClosedInterfacesOf(typeof(ICommandHandler<,>))
                            .InstancePerDependency();
                        break;
                    case Lifetime.InstancePerOwned:
                        throw new NotSupportedException();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        builder.RegisterType<CommandHandlerFactory>().As<ICommandHandlerFactory>().InstancePerLifetimeScope();

        return builder;
    }

    /// <summary>
    /// Registers a decorator for command handlers with the <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="condition">Condition to decide whether the decorator should be applied.</param>
    /// <returns>Current <see cref="CommandHandlerConfiguration"/> instance.</returns>
    public static CommandHandlerConfiguration AddDecorator<TDecorator>(this CommandHandlerConfiguration options, Func<IDecoratorContext, bool>? condition = null) where TDecorator : ICommandHandler
    {
        if (typeof(TDecorator).IsGenericType || typeof(TDecorator).IsGenericTypeDefinition)
            throw new NotSupportedException("Given decorator type is a generic type, use AddGenericDecorator method instead");
        if (typeof(TDecorator).IsAssignableToWithGenerics(typeof(ICommandHandler<>)))
            options.Builder.RegisterDecorator(typeof(TDecorator), typeof(ICommandHandler<>), condition);
        if (typeof(TDecorator).IsAssignableToWithGenerics(typeof(ICommandHandler<,>)))
            options.Builder.RegisterDecorator(typeof(TDecorator), typeof(ICommandHandler<,>), condition);
        else
            throw new NotSupportedException("Given decorator type can't decorate any command handler");

        return options;
    }

    /// <summary>
    /// Registers a generic decorator for command handlers with the <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="decoratorType">Decorator type.</param>
    /// <param name="condition">Condition to decide whether the decorator should be applied.</param>
    /// <returns>Current <see cref="CommandHandlerConfiguration"/> instance</returns>
    public static CommandHandlerConfiguration AddGenericDecorator(this CommandHandlerConfiguration options, Type decoratorType, Func<IDecoratorContext, bool>? condition = null)
    {
        if (!decoratorType.IsGenericType && !decoratorType.IsGenericTypeDefinition)
            throw new NotSupportedException("Given decorator type is not a generic type");
        if (decoratorType.IsAssignableToWithGenerics(typeof(ICommandHandler<>)))
            options.Builder.RegisterGenericDecorator(decoratorType, typeof(ICommandHandler<>), condition);
        if (decoratorType.IsAssignableToWithGenerics(typeof(ICommandHandler<,>)))
            options.Builder.RegisterGenericDecorator(decoratorType, typeof(ICommandHandler<,>), condition);
        else
            throw new NotSupportedException("Given decorator type can't decorate any command handler");

        return options;
    }

    /// <summary>
    /// Registers an adapter for command handlers with the <see cref="ContainerBuilder"/>.
    /// </summary>
    /// <param name="options">Options.</param>
    /// <param name="adapter">Func that used to adapt service to another.</param>
    /// <returns>Current <see cref="CommandHandlerConfiguration"/> instance.</returns>
    public static CommandHandlerConfiguration AddAdapter<TAdapter, THandler>(
        this CommandHandlerConfiguration options, Func<THandler, TAdapter> adapter)
        where THandler : class, ICommandHandler where TAdapter : notnull
    {
        options.Builder.RegisterAdapter(adapter);
        return options;
    }
}
