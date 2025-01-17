using System.Collections.Generic;
using UnityEngine;
using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

/// <summary>
/// Keeping all relevant information about a unit on a scriptable means we can gather and show
/// info on the menu screen, without instantiating the unit prefab.
/// </summary>

[JsonConverter(typeof(JsonSubtypes), "Type")]
public abstract class SkyObject
{

    public string Type { get; set; }
    public string Description;
    public abstract Dictionary<string, string> GetTabularizedData();
    public abstract GameObject GetPrefab(bool isVolumetric);
    public abstract void CalculateAzimuthAltitude(double longitude, double latitude, DateTime time);
    public abstract void AfterSpawn(GameObject gameObject);
    public double Azimuth;
    public double Altitude;
    public string Name;
}
