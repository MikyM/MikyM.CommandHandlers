namespace MikyM.CommandHandlers;

/// <summary>
/// Defines a base command. <b>Used ONLY as a marker interface.</b>
/// </summary>
public interface IBaseCommand
{
}

/// <summary>
/// Defines a base command without a concrete result.
/// </summary>
[PublicAPI]
public interface ICommand : IBaseCommand
{
}

/// <summary>
/// Defines a base command with a concrete result.
/// </summary>
[PublicAPI]
public interface ICommand<TResult> : IBaseCommand
{
}
