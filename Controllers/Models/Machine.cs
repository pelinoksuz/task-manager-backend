public class Machine
{
    public int Id { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string MachineType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Plant { get; set; } = string.Empty;
    public string Line { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
}
