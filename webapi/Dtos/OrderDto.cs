namespace webapi.Dtos;

public class OrderDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CustomerName { get; set; } = "Bek";
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public double Sum { get; set; } = 16590.50;
}