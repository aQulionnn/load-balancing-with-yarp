namespace webapi.LeaderElection.Bully;

public class HeartbeatJob(IServiceProvider serviceProvider, Node node) 
    : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly Node _node = node;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var service = _serviceProvider.GetRequiredService<LeaderElectionService>();
        
        if (!_node.IsLeader && _node.LastHeartbeat == default)
        {
            await service.StartElection(stoppingToken);
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_node.IsLeader)
            {
                await service.SendHeartbeatMessages(stoppingToken);
            }
            else
            {
                if (DateTime.UtcNow - _node.LastHeartbeat > TimeSpan.FromSeconds(35))
                {
                    await service.StartElection(stoppingToken);
                }    
            }
            
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}