namespace webapi.LeaderElection.Bully;

public class LeaderElectionService(Node node, IHttpClientFactory httpClientFactory)  
{
    private readonly Node _node = node;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly List<string> _urls = ["http://asia-api:6001/api/v1/node/health", "http://europe-api:5001/api/v1/node/health"];
    
    public async Task SendHeartbeatMessages(CancellationToken cancellationToken)
    {
        foreach (var url in _urls)
        {
            var client = _httpClientFactory.CreateClient();
            await client.PostAsync(url, null, cancellationToken);
        }
    }

    public async Task StartElection(CancellationToken cancellationToken)
    {
        int count = 0;
        
        foreach (var url in _urls)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync(url, null, cancellationToken);
                if (!response.IsSuccessStatusCode) continue;

                var content = await response.Content.ReadFromJsonAsync<NodeDto>(cancellationToken: cancellationToken);
                if (content != null && content.NodeId > _node.Id)
                    count++;
            }
            catch { }
        }

        if (count == 0) _node.IsLeader = true;
    }
}

public record NodeDto(int NodeId);