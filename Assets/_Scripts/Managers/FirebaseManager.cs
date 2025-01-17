using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseManager : StaticInstance<FirebaseManager>
{
    private DatabaseReference DatabaseReference;
    public List<CelestialBody> ObjectCache = new();

    public bool IsInitialized { get; private set; } = false;
    public bool IsDataLoaded { get; private set; } = false;

    public void Initialize()
    {
        FirebaseApp.LogLevel = LogLevel.Verbose;

        AppOptions options = new AppOptions
        {
            ApiKey = "...",
            AppId = "...",
            ProjectId = "...",
            DatabaseUrl = new System.Uri("..."),
            StorageBucket = "..."
        };


        FirebaseApp.Create(options);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);
                DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                Debug.Log("Firebase initialized with offline support.");
                LoadAllData();
                AttachListener();
                IsInitialized = true;
            }
            else
            {
                Debug.LogError($"Firebase failed to initialize: {task.Result}");
            }
        });
    }

    public List<CelestialBody> GetCelestialBodies()
    {
        return ObjectCache;
    }

    private void LoadAllData()
    {
        DatabaseReference.Child("celestialObjects").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"Failed to fetch celestial objects: {task.Exception}");
                return;
            }

            UpdateLocalCache(task.Result);
        });
    }

    private void AttachListener()
    {
        DatabaseReference.Child("celestialObjects").ValueChanged += OnValueChanged;
    }

    private void OnValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (e.Snapshot.Exists)
        {
            UpdateLocalCache(e.Snapshot);
        }
    }

    private void UpdateLocalCache(DataSnapshot snapshot)
    {
        ObjectCache.Clear();

        foreach (var child in snapshot.Children)
        {
            CelestialBody celestial = JsonUtility.Deserialize<CelestialBody>(child.GetRawJsonValue(), Debug.LogError);
            if (celestial != null)
            {
                ObjectCache.Add(celestial);
            }
        }

        IsDataLoaded = true;
    }

    public IEnumerator WaitForDataLoad()
    {
        while (!IsDataLoaded)
        {
            yield return null;
        }
    }
}
