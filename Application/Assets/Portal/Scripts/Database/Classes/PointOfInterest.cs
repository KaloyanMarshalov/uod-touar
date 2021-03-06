﻿using SQLite4Unity3d;
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
    [NotNull]
    public bool Has360 { get; set; }
    [NotNull]
    public bool HasPortal { get; set; }
    [NotNull]
    public bool HasPedestal { get; set; }
}