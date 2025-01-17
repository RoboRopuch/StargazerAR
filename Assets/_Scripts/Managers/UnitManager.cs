using System;
using System.Collections;
using UnityEngine;


public class UnitManager : StaticInstance<UnitManager>
{
    public float CelestialSphereRadius = 50;
    public GridBuilder Grid;
    public Camera ARCamera;
    public GameObject Enviroment;
    private Quaternion PreviousEnviromentRotation;
    private Quaternion CurrentEnviromentRotation;
    private bool IsRotating;


    void Start()
    {
        PreviousEnviromentRotation = Enviroment.transform.rotation;
        CurrentEnviromentRotation = Enviroment.transform.rotation;
    }

    public void SpawnUnits()
    {
        Grid.DrawGrid(Enviroment.transform, CelestialSphereRadius);
        SpawnCelestialBodies();
    }

    IEnumerator AlignEnviromentWithAvarageNorth()
    {
        yield return new WaitForSeconds(5f);
        float AvgNorth = ContextManager.Instance.AvgCompassHeading;
        UpdateEnvRotation(Quaternion.Euler(0, -AvgNorth, 0), true);

    }

    private void AlignEnviromentWithNorth()
    {
        float initialNorth = ContextManager.Instance.NorthDirection;
        UpdateEnvRotation(Quaternion.Euler(0, -initialNorth, 0), false);
        StartCoroutine(AlignEnviromentWithAvarageNorth());
    }

    private void UpdateEnvRotation(Quaternion newRotation, bool smoothlyRotate)
    {
        PreviousEnviromentRotation = CurrentEnviromentRotation;
        CurrentEnviromentRotation = newRotation;

        if (!smoothlyRotate)
        {
            Enviroment.transform.rotation = CurrentEnviromentRotation;
        }
        else
        {
            IsRotating = true;
        }
    }

    public void ForceRotationSync()
    {
        Enviroment.transform.rotation = CurrentEnviromentRotation;
    }

    void SpawnCelestialBody(CelestialBody celestial)
    {
        Vector3 cartesianPosition = ARCamera.transform.position + Helpers.GeographicToCartesian(celestial.Azimuth, celestial.Altitude, CelestialSphereRadius);
        GameObject celestialPrefab = celestial.GetPrefab(false);
        GameObject spawn = GameObject.Instantiate(celestialPrefab, cartesianPosition, Quaternion.identity);
        spawn.name = celestial.Name;
        Helpers.GetInteractable(spawn).data = celestial;
        celestial.AfterSpawn(spawn);
        spawn.transform.SetParent(Enviroment.transform);
    }

    void SpawnCelestialBodies()
    {

        var celestialBodiesToSpawn = FirebaseManager.Instance.ObjectCache;
        (double userLatitude, double userLongitude) = ContextManager.Instance.UserLocation.GetCoordinates();
        DateTime timeOfObservation = DateTime.Now;

        foreach (var celestial in celestialBodiesToSpawn)
        {
            try
            {
                celestial.CalculateAzimuthAltitude(userLongitude, userLatitude, timeOfObservation);

                if (celestial.Altitude < -10)
                {
                    continue;
                }
                else
                {
                    SpawnCelestialBody(celestial);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Was not able to spawn, skip object: {ex}");
                continue;
            }

        }

        AlignEnviromentWithNorth();

    }

    void Update()
    {
        if (IsRotating && PreviousEnviromentRotation != CurrentEnviromentRotation)
        {
            Enviroment.transform.rotation = Quaternion.Lerp(Enviroment.transform.rotation, CurrentEnviromentRotation, Time.deltaTime * 5f);
            if (Quaternion.Angle(Enviroment.transform.rotation, CurrentEnviromentRotation) < 0.01f)
            {
                IsRotating = false;
            }
        }

    }


}