namespace webapi.LeaderElection.Bully;

public class Node
{
    public int Id { get; private init; }
    public DateTime LastHeartbeat { get; set; } = DateTime.Now;
    public bool IsLeader { get; set; } = false;

    public Node()
    {
        var nodeId = Environment.GetEnvironmentVariable("NODE_ID") ?? "0";
        Id = int.Parse(nodeId);
    }
}