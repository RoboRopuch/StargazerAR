using System;
using System.Collections.Generic;
using CosineKitty;
using UnityEngine;

public class Planet : CelestialBody
{

    public double Radius;
    public float Density;
    public float Gravity;
    public float Mass;
    public int MassExponent;

    public override void AfterSpawn(GameObject gameObject)
    {
        gameObject.transform.localScale = Vector3.one * Helpers.MapRange((float)Radius, 1180, 695508, 1, 2);
    }

    public override void CalculateAzimuthAltitude(double longitude, double latitude, DateTime time)
    {
        Helpers.Map(Name, out Body cosnineKittyBody);
        AstroTime astroTime = new(time);
        var (azimuth, altitude) = Helpers.CalculateAzimuthAltitude(cosnineKittyBody, latitude, longitude, astroTime);

        Azimuth = azimuth;
        Altitude = altitude;
    }

    public override GameObject GetPrefab(bool isVolumetric)
    {
        return Helpers.GetPrefab(Name);
    }


    public override Dictionary<string, string> GetTabularizedData()
    {
        var KeyValue = new Dictionary<string, string>
        {
            { "Radius", $"{Radius} km" },
            { "Density", $"{Density} g/cm\u00B3"},
            { "Gravity", $"{Gravity} m/s" },
            { "Mass", Helpers.FormatMass(Mass, MassExponent) },

        };

        return KeyValue;
    }

}
