public class AccessResponseDto
{
    public string Role { get; set; } = string.Empty;
    public Dictionary<string, string> Permissions { get; set; } = new();
}