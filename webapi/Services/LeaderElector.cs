using RedLockNet;

namespace webapi.Services;

public class LeaderElector(IDistributedLockFactory factory)
{
    private readonly IDistributedLockFactory _factory = factory;
    private IRedLock _lock;

    public async Task<bool> IsLeaderAsync(CancellationToken cancellationToken)
    {
        if (_lock?.IsAcquired != true) 
            _lock = await _factory.CreateLockAsync("leader-lock", TimeSpan.FromSeconds(10));
        
        return _lock.IsAcquired;
    }
}