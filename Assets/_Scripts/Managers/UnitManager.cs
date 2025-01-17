using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// An example of a scene-specific manager grabbing resources from the resource system
/// Scene-specific managers are things like grid managers, unit managers, environment managers etc
/// </summary>
public class UnitManager : StaticInstance<UnitManager>
{
    public float CelestialSphereRadius = 50;
    public GridBuilder grid;
    public Camera ARCamera;
    public GameObject enviroment;
    private Quaternion previousEnviromentRotation;
    private Quaternion currentEnviromentRotation;
    private bool isSmoothlyRotating;


    void Start()
    {
        previousEnviromentRotation = enviroment.transform.rotation;
        currentEnviromentRotation = enviroment.transform.rotation;
    }

    public void SpawnHeroes()
    {
        grid.DrawGrid(enviroment.transform, CelestialSphereRadius);
        Debug.Log("I am here");
        SpawnUnits();
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
        previousEnviromentRotation = currentEnviromentRotation;
        currentEnviromentRotation = newRotation;

        if (!smoothlyRotate)
        {
            enviroment.transform.rotation = currentEnviromentRotation;
        }
        else
        {
            isSmoothlyRotating = true;
        }
    }

    public void ForceRotationSync()
    {
        enviroment.transform.rotation = currentEnviromentRotation;
    }

    void SpawnUnits()
    {
        var celestials = FirebaseManager.Instance.ObjectCache;
        (double latitude, double longitude) = ContextManager.Instance.UserLocation.GetCoordinates();
        DateTime time = DateTime.Now;

        foreach (var celestial in celestials)
        {
            try
            {
                celestial.CalculateAzimuthAltitude(longitude, latitude, time);
                if (celestial.Altitude < -10)
                {
                    continue;
                }
                else
                {
                    Vector3 cartesianPosition = ARCamera.transform.position + Helpers.CalculatePosition(celestial.Azimuth, celestial.Altitude, CelestialSphereRadius);
                    GameObject celestialPrefab = celestial.GetPrefab(false);
                    var spawn = GameObject.Instantiate(celestialPrefab, cartesianPosition, Quaternion.identity);
                    spawn.name = celestial.Name;
                    var interactable = spawn.GetComponent<Interactable>();
                    interactable.data = celestial;
                    celestial.AfterSpawn(spawn);
                    spawn.transform.SetParent(enviroment.transform);
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
        if (isSmoothlyRotating && previousEnviromentRotation != currentEnviromentRotation)
        {
            enviroment.transform.rotation = Quaternion.Lerp(enviroment.transform.rotation, currentEnviromentRotation, Time.deltaTime * 5f);
            if (Quaternion.Angle(enviroment.transform.rotation, currentEnviromentRotation) < 0.01f)
            {
                isSmoothlyRotating = false;
            }
        }

    }


}