
using SQLite4Unity3d;

public class Route 
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
    [NotNull]
    public string ColourHex { get; set; }
    [NotNull]
    public string MapboxRouteJSON { get; set; }
}