using System;
using System.Collections.Generic;
using System.IO;
using CosineKitty;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// A static class for general helpful methods
/// </summary>
///
public static class Helpers
{
    public static int MovingAvgWindow = 5;

    private static readonly Dictionary<string, Body> nameBodyMapping = new()
    {
        { "Sun", Body.Sun },
        { "Mercury", Body.Mercury },
        { "Venus", Body.Venus },
        { "Mars", Body.Mars },
        { "Jupiter", Body.Jupiter },
        { "Saturn", Body.Saturn },
        { "Uranus", Body.Uranus },
        { "Neptune", Body.Neptune },
        { "Pluto", Body.Pluto },
        { "Moon", Body.Moon },
    };

    public static bool Map(string name, out Body body)
    {
        if (!nameBodyMapping.TryGetValue(name, out body))
        {
            Debug.LogWarning($"The name '{name}' does not correspond to a valid celestial body.");
            return false;
        }
        return true;
    }


    public static (double azimuth, double altitude) CalculateAzimuthAltitude(
        double rightAscension, double declination, double longitude, double latitude, DateTime time)
    {
        float lst = CalculateLocalSiderealTime(time, longitude);
        float hourAngle = CalculateHourAngle(lst, rightAscension);
        double altitude = CalculateAltitude(declination, latitude, hourAngle);
        double azimuth = CalculateAzimuth(declination, latitude, altitude, hourAngle);
        azimuth = NormalizeAngle(azimuth);
        return (azimuth, altitude);
    }

    public static (double azimuth, double altitude) CalculateAzimuthAltitude(Body body, double latitude, double longitude, AstroTime time)
    {
        Observer observer = new(latitude, longitude, 100);

        Equatorial equ_ofdate = Astronomy.Equator(body, time, observer, EquatorEpoch.OfDate, Aberration.Corrected);
        Topocentric hor = Astronomy.Horizon(time, observer, equ_ofdate.ra, equ_ofdate.dec, Refraction.Normal);
        return (hor.azimuth, hor.altitude);
    }


    public static float CalculateHourAngle(float lst, double rightAscension)
    {
        float hourAngle = lst - (float)(rightAscension * 15);
        return NormalizeAngle(hourAngle);
    }

    public static Vector3 CalculatePosition(double azimuth, double altitude, float distance)
    {
        float azRad = Mathf.Deg2Rad * (float)azimuth;
        float altRad = Mathf.Deg2Rad * (float)altitude;
        float x = distance * Mathf.Cos(altRad) * Mathf.Sin(azRad);
        float y = distance * Mathf.Sin(altRad);
        float z = distance * Mathf.Cos(altRad) * Mathf.Cos(azRad);
        return new Vector3(x, y, z);
    }

    public static float CalculateLocalSiderealTime(DateTime time, double longitude)
    {
        double julianDay = (time.ToUniversalTime() - new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc)).TotalDays + 2451545.0;
        double gst = 280.46061837 + 360.98564736629 * (julianDay - 2451545.0);

        gst = NormalizeAngle(gst);

        float lst = (float)((gst + longitude) % 360);
        return NormalizeAngle(lst);
    }

    public static double CalculateAltitude(double declination, double latitude, double hourAngle)
    {
        double decRad = Math.PI / 180 * declination;
        double latRad = Math.PI / 180 * latitude;
        double haRad = Math.PI / 180 * hourAngle;

        double sinAlt = Math.Sin(decRad) * Math.Sin(latRad) +
                        Math.Cos(decRad) * Math.Cos(latRad) * Math.Cos(haRad);

        return Math.Asin(sinAlt) * 180 / Math.PI;
    }

    public static string Deg2Str(float number, int digitsAfterDecimal = 2)
    {
        double roundedValue = Math.Round((double)number, digitsAfterDecimal);
        return roundedValue.ToString($"F{digitsAfterDecimal}") + "°";
    }

    public static string Deg2Str(double number, int digitsAfterDecimal = 2)
    {
        double roundedValue = Math.Round((double)number, digitsAfterDecimal);
        return roundedValue.ToString($"F{digitsAfterDecimal}") + "°";
    }

    public static double CalculateAzimuth(double declination, double latitude, double altitude, double hourAngle)
    {
        double decRad = Math.PI / 180 * declination; // Convert to radians
        double latRad = Math.PI / 180 * latitude;    // Convert to radians
        double altRad = Math.PI / 180 * altitude;    // Convert to radians
        double haRad = Math.PI / 180 * hourAngle;    // Convert to radians

        double cosAz = (Math.Sin(decRad) - Math.Sin(altRad) * Math.Sin(latRad)) /
                       (Math.Cos(altRad) * Math.Cos(latRad));
        double azRadians = Math.Acos(cosAz); // Compute azimuth in radians

        if (Math.Sin(haRad) > 0)
            azRadians = 2 * Math.PI - azRadians;

        return azRadians * 180 / Math.PI; // Convert back to degrees
    }


    public static float MapRange(float value, float originalMin, float originalMax, float targetMin, float targetMax)
    {
        if (originalMax - originalMin == 0)
        {
            Debug.LogWarning("Original range has zero size. Returning targetMin.");
            return targetMin;
        }

        return targetMin + (value - originalMin) * (targetMax - targetMin) / (originalMax - originalMin);
    }

    public static float NormalizeAngle(double angle)
    {
        return (float)((angle + 360) % 360);
    }

    public static GameObject GetPrefab(string prefabName)
    {
        return Resources.Load<GameObject>("Prefabs/" + $"{prefabName}");
    }


    public static SkyObject GetAssociatedObject(RaycastHit objectToGetDataFrom)
    {
        SkyObject data = objectToGetDataFrom.transform.GetComponent<Interactable>().data;
        Debug.Log(data);
        return data;
    }


    public static GameObject GetGameObject(RaycastHit objectHit)
    {
        return objectHit.collider.gameObject;
    }


    public static string FormatMass(double baseValue, int exponent)
    {
        return $"{baseValue:F2} × 10^{exponent}";
    }


}


public static class JsonUtility
{
    public static T Deserialize<T>(string json, Action<string> onError = null)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            onError?.Invoke($"Error deserializing JSON: {ex.Message}");
            return default;
        }
    }

    public static string Serialize<T>(T obj, Action<string> onError = null)
    {
        try
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
        catch (Exception ex)
        {
            onError?.Invoke($"Error serializing object: {ex.Message}");
            return null;
        }
    }

    public static void SaveToFile(string filePath, string data, Action<string> onError = null)
    {
        try
        {
            File.WriteAllText(filePath, data);
        }
        catch (Exception ex)
        {
            onError?.Invoke($"Error saving to file: {ex.Message}");
        }
    }

    public static string LoadFromFile(string filePath, Action<string> onError = null)
    {
        try
        {
            return File.Exists(filePath) ? File.ReadAllText(filePath) : null;
        }
        catch (Exception ex)
        {
            onError?.Invoke($"Error loading file: {ex.Message}");
            return null;
        }
    }
}