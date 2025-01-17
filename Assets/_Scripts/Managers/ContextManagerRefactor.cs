// // Filename: ContextManager.cs
// // Author: Magdlena Dudek
// // Description: Controls process of storing contextual infromation about user. 

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// public class GeographicCoordinates
// {
//     public double Latitude { get; set; }
//     public double Longitude { get; set; }

//     public GeographicCoordinates(double latitude, double longitude)
//     {
//         Latitude = latitude;
//         Longitude = longitude;
//     }

//     public (double Latitude, double Longitude) GetCoordinates()
//     {
//         return (Latitude, Longitude);
//     }
// }


// public abstract class Service
// {
//     public string Name { get; }
//     public int WaitTime { get; }
//     public static Action<string> OnInitializationFailed;
//     public static Action<string> OnTimeout;
//     public static Action OnComplete;

//     abstract public void StartInitialization();
//     abstract public bool IsInitializing();
//     abstract public bool CheckIfFailed();


//     public Service(string name, int waitTime, Action<string> onInitializationFailed, Action onComplete, Action<string> onTimeout)
//     {
//         Name = name;
//         WaitTime = waitTime;
//         OnInitializationFailed = onInitializationFailed;
//         OnTimeout = onTimeout;
//         OnComplete = onComplete;
//     }

//     public IEnumerator CoordinateInitialization()
//     {
//         StartInitialization();
//         int localWaitTime = WaitTime;

//         string timeoutMessage = $"{Name} initalization timeout";
//         string failedInitializationMessage = $"{Name} initalization failed";

//         while (IsInitializing() && localWaitTime > 0)
//         {
//             yield return new WaitForSeconds(1);
//             localWaitTime--;
//         }

//         if (localWaitTime < 1)
//         {
//             OnTimeout.Invoke(timeoutMessage);
//             yield break;
//         }

//         if (CheckIfFailed())
//         {
//             OnInitializationFailed.Invoke(failedInitializationMessage);
//             yield break;
//         }

//         OnComplete.Invoke();
//     }
// }

// class LocationService : Service
// {
//     public LocationService(string name, int waitTime, Action<string> onInitializationFailed, Action onComplete, Action<string> onTimeout)
//     : base(name, waitTime, onInitializationFailed, onComplete, onTimeout)
//     { }

//     public override bool CheckIfFailed()
//     {
//         return Input.location.status == LocationServiceStatus.Failed;
//     }

//     public override bool IsInitializing()
//     {
//         return Input.location.status == LocationServiceStatus.Initializing;
//     }

//     public override void StartInitialization()
//     {
//         Input.location.Start();
//     }
// }

// class CompassService : Service
// {
//     public CompassService(string name, int waitTime, Action<string> onInitializationFailed, Action onComplete, Action<string> onTimeout)
//     : base(name, waitTime, onInitializationFailed, onComplete, onTimeout)
//     { }

//     public override bool CheckIfFailed()
//     {
//         return Input.compass.trueHeading == 0;
//     }

//     public override bool IsInitializing()
//     {
//         return Input.compass.trueHeading == 0;
//     }

//     public override void StartInitialization()
//     {
//         // We can intiialize compass only after location servise is up and running
//         if (Input.location.status == LocationServiceStatus.Running)
//         {
//             Input.compass.enabled = true;
//         }
//         else
//         {
//             Debug.Log("Tried to initialize compass bfore localization service. You need to make sure localization service is running before.");
//         }

//     }
// }

// public class ContextManager : StaticInstance<ContextManager>
// {
//     public event Action<GeographicCoordinates> OnLocationChange;
//     public event Action<double> OnComapssChange;

//     public bool IsLocationEnabledByUser = false;
//     public bool IsLocationServiceInitialized = false;
//     public bool IsComapssInitialized = false;

//     public float NorthDirection;
//     public GeographicCoordinates UserLocation { get; private set; }
//     public Vector3 GyroAttitude { get; private set; }

//     public bool CheckLocationPermission()
//     {
//         return IsLocationEnabledByUser = Input.location.isEnabledByUser;
//     }

//     public void ActivateSensors(Action onComplete, Action<string> onError)
//     {
//         StartCoroutine(InitializeSensors(onComplete, onError));
//     }

//     private IEnumerator InitializeSensors(Action onComplete, Action<string> onError)
//     {
//         LocationService locationService = new("Location Service", 10, onError, () => { IsLocationServiceInitialized = true; }, onError);
//         CompassService compassService = new("Compass", 10, onError, () => { IsComapssInitialized = true; }, onError);

//         yield return StartCoroutine(locationService.CoordinateInitialization());
//         yield return StartCoroutine(compassService.CoordinateInitialization());

//         onComplete?.Invoke();
//     }

//     public void UpdateLocationReading()
//     {
//         if (Input.location.status == LocationServiceStatus.Running)
//         {
//             UserLocation = new GeographicCoordinates(Input.location.lastData.latitude, Input.location.lastData.longitude);
//             OnLocationChange.Invoke(UserLocation);
//         }
//     }

//     public float AvgCompassHeading = 0;
//     public int Index = 0;
//     public float Value = 0;
//     public List<float> Readings = Enumerable.Repeat(0f, Helpers.MovingAvgWindow).ToList();
//     public float Sum = 0;


//     void UpdateCompassReding()
//     {

//         if (!IsComapssInitialized) return;
//         NorthDirection = Input.compass.trueHeading;

//     }

//     void UpdateMovingAvarage()
//     {
//         if (!IsComapssInitialized) return;

//         Sum -= Readings[Index];
//         Value = NorthDirection;
//         Readings[Index] = Value;
//         Sum += Value;
//         Index = (Index + 1) % Helpers.MovingAvgWindow;

//         AvgCompassHeading = Sum / Helpers.MovingAvgWindow;
//     }


//     void Update()
//     {
//         UpdateCompassReding();
//         UpdateMovingAvarage();
//     }

// }


