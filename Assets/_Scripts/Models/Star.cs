using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;



public class Star : SkyObject
{

    public double RightAscension;
    public double Declination;
    public string CommonName;
    public string Constellation;
    public string BayerDesignation;
    public double Magnitude;



    public override GameObject GetPrefab(bool isVolumetric)
    {
        string prefabName = isVolumetric ? "SphereStar" : "SpriteStar";
        return Helpers.GetPrefab(prefabName);
    }


    public override Dictionary<string, string> GetTabularizedData()
    {
        var KeyValue = new Dictionary<string, string>
        {
            { "Right Ascention", $"{RightAscension} °" },
            { "Declination", $"{Declination} °" },
            { "Magnitude", $"{Magnitude}" },
            { "Constllation", $"{Constellation}" }
        };

        return KeyValue;
    }

    public override void CalculateAzimuthAltitude(double longitude, double latitude, DateTime time)
    {
        var (azimuth, altitude) = Helpers.CalculateAzimuthAltitude(RightAscension, Declination, longitude, latitude, time);

        Azimuth = azimuth;
        Altitude = altitude;
    }

    public override void AfterSpawn(GameObject gameObject)
    {
        gameObject.transform.LookAt(UnitManager.Instance.ARCamera.transform, Vector3.down);
        gameObject.transform.localScale = Vector3.one * Helpers.MapRange((float)Magnitude, 0, 4, 0.4f, 1);

    }

    // public override GameObject SpawnSelf(Vector3 position)
    // {

    //     var spawned = GameObject.Instantiate(SimplifiedPrefab, position, Quaternion.identity);
    //     spawned.name = Name;
    //     spawned.transform.LookAt(UnitManager.Instance.ARCamera.transform, Vector3.down);
    //     spawned.transform.localScale = Vector3.one * Helpers.MapRange((float)Magnitude, 0, 4, 0.4f, 1);

    //     return spawned;
    // }
}