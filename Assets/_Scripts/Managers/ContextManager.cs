// Filename: ContextManager.cs
// Author: Magdlena Dudek
// Description: Controls process of storing contextual infromation about user. 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeographicCoordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public GeographicCoordinates(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public (double Latitude, double Longitude) GetCoordinates()
    {
        return (Latitude, Longitude);
    }
}


public class ContextManager : StaticInstance<ContextManager>
{
    public static event Action<GeographicCoordinates> OnLocationChange;
    public event Action<double> OnComapssChange;

    public bool IsLocationEnabledByUser = false;
    public bool IsLocationServiceInitialized = false;
    public bool IsComapssInitialized = false;
    private bool IsGyroInitialized = false;

    public float NorthDirection;
    public GeographicCoordinates UserLocation { get; private set; } = new GeographicCoordinates(0, 0);
    public Vector3 GyroAttitude { get; private set; }

    public bool CheckLocationPermission()
    {
        return IsLocationEnabledByUser = Input.location.isEnabledByUser;
    }

    public void ActivateSensors(Action onComplete, Action<string> onError)
    {
        StartCoroutine(InitializeSensors(onComplete, onError));
    }

    private IEnumerator InitializeSensors(Action onComplete, Action<string> onError)
    {

        yield return StartCoroutine(StartLocationService(onError));
        yield return StartCoroutine(StartCompass(onError));
        yield return StartCoroutine(StartGyroscope(onError));

        onComplete?.Invoke();
    }

    private IEnumerator StartLocationService(Action<string> onError)
    {
        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            onError?.Invoke("Location service initialization timed out.");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            onError?.Invoke("Unable to initialize location service.");
            yield break;
        }

        IsLocationServiceInitialized = true;
        Debug.Log("Location service started successfully.");
    }

    /// <summary>
    /// Starts the compass system.
    /// </summary>
    private IEnumerator StartCompass(Action<string> onError)
    {
        Input.compass.enabled = true;

        int maxWait = 20;
        while (Input.compass.trueHeading == 0 && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            onError?.Invoke("Compass initialization timed out.");
            yield break;
        }

        if (Input.compass.trueHeading == 0)
        {
            onError?.Invoke("Compass initialization failed.");
            yield break;
        }

        IsComapssInitialized = true;
    }

    /// <summary>
    /// Starts the gyroscope system.
    /// </summary>
    private IEnumerator StartGyroscope(Action<string> onError)
    {
        Input.gyro.enabled = true;

        int maxWait = 20;

        while (maxWait > 0)
        {
            if (Input.gyro.attitude != Quaternion.identity)
                break;

            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            onError?.Invoke("Gyroscope initialization failed.");
            yield break;
        }

        IsGyroInitialized = true;
    }
    public void UpdateLocationReading()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            UserLocation = new GeographicCoordinates(Input.location.lastData.latitude, Input.location.lastData.longitude);
            OnLocationChange.Invoke(UserLocation);
        }
    }

    void UpdateGyroReading()
    {
        if (Input.gyro.enabled)
        {
            GyroAttitude = Input.gyro.attitude.eulerAngles;
        }
    }


    public float AvgCompassHeading = 0;
    public int Index = 0;
    public float Value = 0;
    public List<float> Readings = Enumerable.Repeat(0f, Helpers.MovingAvgWindow).ToList();
    public float Sum = 0;


    void UpdateCompassReding()
    {

        if (!IsComapssInitialized) return;
        NorthDirection = Input.compass.trueHeading;

    }

    void UpdateMovingAvarage()
    {
        if (!IsComapssInitialized) return;

        Sum -= Readings[Index];
        Value = NorthDirection;
        Readings[Index] = Value;
        Sum += Value;
        Index = (Index + 1) % Helpers.MovingAvgWindow;

        AvgCompassHeading = Sum / Helpers.MovingAvgWindow;
    }


    void Update()
    {
        UpdateCompassReding();
        UpdateMovingAvarage();
        UpdateGyroReading();
    }


}


