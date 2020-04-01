using SQLite4Unity3d;
using UnityEngine;
public class PointOfInterest
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [NotNull]
    public string Name { get; set; }
    public string Description { get; set; }
    [NotNull]
    public float Latitude { get; set; }
    [NotNull]
    public float Longitude { get; set; }
    [NotNull]
    public bool Visited { get; set; }
    public bool Has360 { get; set; }
    public bool HasPortal { get; set; }
}