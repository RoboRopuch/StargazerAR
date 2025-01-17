using UnityEngine;
using System.Collections.Generic;

public class GridBuilder : MonoBehaviour
{
    [Header("Grid Configuration")]
    [Range(1, 360)] public int azimuthDivisions = 36;
    [Range(1, 90)] public int altitudeDivisions = 9;
    public float sphereRadius = 10f;
    public Material gridMaterial;
    public Transform labelParent;
    public GameObject labelPrefab;

    private GameObject gridObject;

    public void DrawGrid(Transform parent, float celestialSpere)
    {
        sphereRadius = celestialSpere;
        Mesh gridMesh = GenerateGridMesh();
        CreateGridObject(gridMesh, parent);
        AddAzimuthLabels(parent);
        AddAltitudeLabels(parent);
    }


    private void CreateGridObject(Mesh gridMesh, Transform parent)
    {
        if (gridObject != null) Destroy(gridObject);

        gridObject = new GameObject("AzimuthAltitudeGrid");
        gridObject.transform.SetParent(parent, worldPositionStays: false);

        MeshFilter mf = gridObject.AddComponent<MeshFilter>();
        MeshRenderer mr = gridObject.AddComponent<MeshRenderer>();

        mf.mesh = gridMesh;
        mr.material = gridMaterial;
    }


    private Mesh GenerateGridMesh()
    {
        Mesh mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        GenerateAzimuthLines(vertices, indices);
        GenerateAltitudeLines(vertices, indices);

        mesh.vertices = vertices.ToArray();
        mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);

        return mesh;
    }

    private void GenerateAzimuthLines(List<Vector3> vertices, List<int> indices)
    {
        for (int i = 0; i < azimuthDivisions; i++)
        {
            float azimuth = i * (360f / azimuthDivisions);
            float radAzimuth = Mathf.Deg2Rad * azimuth;

            int startVertexIndex = vertices.Count;
            for (int j = 0; j <= 180; j++)
            {
                float latitude = -90f + (j * (180f / altitudeDivisions));
                vertices.Add(GenerateLatitudeVertex(radAzimuth, latitude));

                if (j > 0)
                {
                    indices.Add(startVertexIndex + j - 1);
                    indices.Add(startVertexIndex + j);
                }
            }
        }
    }

    private void GenerateAltitudeLines(List<Vector3> vertices, List<int> indices)
    {
        for (int i = 0; i <= altitudeDivisions; i++)
        {
            float altitude = i * (90f / altitudeDivisions);
            float radAltitude = Mathf.Deg2Rad * altitude;

            int startVertexIndex = vertices.Count;
            for (int j = 0; j <= 360; j++)
            {
                float azimuth = j;
                float radAzimuth = Mathf.Deg2Rad * azimuth;
                vertices.Add(GenerateAzimuthVertex(radAltitude, radAzimuth));

                if (j > 0)
                {
                    indices.Add(startVertexIndex + j - 1);
                    indices.Add(startVertexIndex + j);
                }
            }
        }
    }

    private Vector3 GenerateLatitudeVertex(float azimuthRadians, float latitudeDegrees)
    {
        float latRadians = Mathf.Deg2Rad * latitudeDegrees;

        return new Vector3(
            sphereRadius * Mathf.Cos(latRadians) * Mathf.Cos(azimuthRadians),
            sphereRadius * Mathf.Sin(latRadians),
            sphereRadius * Mathf.Cos(latRadians) * Mathf.Sin(azimuthRadians)
        );
    }

    private Vector3 GenerateAzimuthVertex(float altitudeRadians, float azimuthRadians)
    {
        return new Vector3(
            sphereRadius * Mathf.Cos(altitudeRadians) * Mathf.Cos(azimuthRadians),
            sphereRadius * Mathf.Sin(altitudeRadians),
            sphereRadius * Mathf.Cos(altitudeRadians) * Mathf.Sin(azimuthRadians)
        );
    }

    private void AddAzimuthLabels(Transform parent)
    {
        if (labelPrefab == null) return;

        float offset = -1f;

        for (int i = 0; i < azimuthDivisions; i++)
        {
            float azimuth = i * (360f / azimuthDivisions);
            float radAzimuth = Mathf.Deg2Rad * azimuth;
            Vector3 position = new Vector3(
                (sphereRadius + offset) * Mathf.Sin(radAzimuth),
                0,
                (sphereRadius + offset) * Mathf.Cos(radAzimuth)
            );

            LabelManager.Instance.AttachLabel(position, $"{Mathf.RoundToInt(azimuth)}°", transform);

        }
    }

    private void AddAltitudeLabels(Transform parent)
    {
        if (labelPrefab == null) return;

        float offset = -1f;

        for (int i = 1; i <= altitudeDivisions; i++)
        {
            float altitude = i * (90f / altitudeDivisions);
            float radAltitude = Mathf.Deg2Rad * altitude;

            Vector3 position = new Vector3(
                0,
                (sphereRadius + offset) * Mathf.Sin(radAltitude),
                (sphereRadius + offset) * Mathf.Cos(radAltitude)
            );


            LabelManager.Instance.AttachLabel(position, $"{Mathf.RoundToInt(altitude)}°", transform);
        }
    }
}
