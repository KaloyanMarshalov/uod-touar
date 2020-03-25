using Mapbox.Utils;
using System;

public static class DistanceCalculator 
{
    private static double convertToRadians(double angle)
    {
        return (Math.PI / 180) * angle;
    }

    //Calculate the distance between two points in meters
    public static double calculateDistance(Vector2d position1, Vector2d position2)
    {
        double R = 6371;

        double dLat = convertToRadians(position2.x - position1.x);
        double dLong = convertToRadians(position2.y - position1.y);

        var h1 = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                  Math.Cos(convertToRadians(position1.x)) * Math.Cos(convertToRadians(position2.x)) *
                  Math.Sin(dLong / 2) * Math.Sin(dLong / 2);
        var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

        return R * h2 * 1000;
    }
}