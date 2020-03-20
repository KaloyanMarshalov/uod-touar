using SQLite4Unity3d;
public class POI_Route
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [NotNull]
    public int RouteId { get; set; }
    [NotNull]
    public int PointOfInterestId { get; set; }
}