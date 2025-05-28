using webapi.Services;

namespace webapi.BackgroundTasks;

public class LoggingLeaderJob(LeaderElector leaderElector, ILogger<LoggingLeaderJob> logger) 
    : BackgroundService
{
    private readonly LeaderElector _leaderElector = leaderElector;
    private readonly ILogger<LoggingLeaderJob> _logger = logger;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (await _leaderElector.IsLeaderAsync(stoppingToken))
            {
                _logger.LogInformation($"{Environment.MachineName} is leader");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

}