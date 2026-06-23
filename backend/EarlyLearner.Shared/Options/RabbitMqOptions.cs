using System.ComponentModel.DataAnnotations;

namespace EarlyLearner.Shared.Options;


/// <summary>
/// Configures the MassTransit RabbitMQ transport used for live operational messaging.
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SECTION_NAME = "RabbitMq";

    /// <summary>
    /// Gets the RabbitMQ host URI used by the API and worker.
    /// </summary>
    [Required] public string HostUri { get; init; } = "rabbitmq://localhost";

    /// <summary>
    /// Gets the username used to authenticate with RabbitMQ.
    /// </summary>
    [Required] public string Username { get; init; } = "guest";

    /// <summary>
    /// Gets the password used to authenticate with RabbitMQ.
    /// </summary>
    [Required] public string Password { get; init; } = "guest";

    /// <summary>
    /// Gets the number of messages the transport can prefetch for a receive endpoint.
    /// </summary>
    public int? PrefetchCount { get; init; } = 1;

    /// <summary>
    /// Gets the maximum number of messages processed concurrently by receive endpoints.
    /// </summary>
    public int? ConcurrentMessageLimit { get; init; } = 1;

    /// <summary>
    /// Gets the message processing timeout in seconds.
    /// </summary>
    public int? TimeoutSeconds { get; init; } = 60;
}
