#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using GameLib;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;
using Math = UnityEngine.ProBuilder.Math;

public class PrintFacesInfo : MonoBehaviour
{
    [Serializable]
    public class FaceSetup
    {
        public Face Face;
        public List<UnityEvent> Handlers;
    }

    public ProBuilderMesh Mesh;
    public List<FaceSetup> FacesSetup;
    public Camera ViewCamera;

    void Start()
    {

    }

    [Button()]
    void AddFace()
    {
        var faces = Mesh.GetSelectedFaces();
        if (faces == null)
            return;
        if (faces.Length != 1)
        {
            Debug.Log("Should be one face selected for Adding");
            return;
        }

        if (FacesSetup == null)
            FacesSetup = new List<FaceSetup>();
        FacesSetup.Add(new FaceSetup { Face = faces[0] });
    }

    [Button()]
    void SelectAllActiveFaces()
    {
        var faces = FacesSetup.Select(x => x.Face);
        Mesh.ClearSelection();
        Mesh.SetSelectedFaces(faces);
        Debug.Log($"{faces.Count()} selected");
    }

    [Button()]
    void CheckSelectedFace()
    {
        var faces = Mesh.GetSelectedFaces();
        if (faces == null)
            return;
        if (faces.Length != 1)
        {
            Debug.Log("Should be one face selected for Print Info");
            return;
        }

        var selectedFace = faces[0];
        var index = 0;

        foreach (var faceSetup in FacesSetup)
        {
            if (faceSetup.Face == selectedFace)
            {
                Debug.Log($"Configuration for selected face: {index}");

            }
            ++index;
        }
    }

    [Button()]
    void PrintInfo()
    {
        Debug.Log($">>>>> Mesh {name} ");
        Debug.Log($"Faces count: {Mesh.faceCount}");
        Debug.Log($"Triangles count: {Mesh.triangleCount}");
        foreach (var meshFace in Mesh.faces)
        {
            Debug.Log(">>> Face ");

            Debug.Log($"is quad: {meshFace.IsQuad()}");
            Debug.Log($"normal: {Math.Normal(Mesh, meshFace)}");
            Debug.Log($"triangle indices count {meshFace.indexes.Count}");
            Debug.Log($"triangles count: {meshFace.distinctIndexes}");
        }
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var faceSetup in FacesSetup)
        {
            // Draw normal
            var normal = Math.Normal(Mesh, faceSetup.Face);
            var faceCenter = Math.Average(Mesh.positions, faceSetup.Face.indexes);

            var camDot = Vector3.Dot(transform.TransformDirection(normal).normalized, ViewCamera.transform.forward.normalized);
            Gizmos.color = Color.Lerp(Color.blue, Color.red, camDot);
            
            //Handles.Label(transform.TransformPoint(faceCenter), $"{camDot}");
            Gizmos.DrawLine(faceCenter, faceCenter + normal);

            // Calculate area
            float area = CalculateSurfaceArea(Mesh, faceSetup.Face.indexes);

            // Calculate screen space area
            float areaFromCam = CalculateSurfaceAreaFromCameraView(Mesh, faceSetup.Face.indexes, area);
            Handles.color = Color.black;
            Handles.Label(transform.TransformPoint(faceCenter), $"{area:F}|{areaFromCam}");
        }
    }
#endif

    float CalculateSurfaceArea(ProBuilderMesh mesh, IList<int> triangles)
    {
        double sum = 0.0;

        for (int i = 0; i < triangles.Count; i += 3)
        {
            Vector3 corner = Mesh.positions[triangles[i]];
            Vector3 a = Mesh.positions[triangles[i + 1]] - corner;
            Vector3 b = Mesh.positions[triangles[i + 2]] - corner;

            sum += Vector3.Cross(a, b).magnitude;
        }

        return (float)(sum / 2.0);
    }

    float CalculateSurfaceAreaFromCameraView(ProBuilderMesh mesh, IList<int> triangles, float localArea)
    {
        double sum = 0.0;
        //Gizmos.matrix = transform.localToWorldMatrix;

        for (int i = 0; i < triangles.Count; i += 3)
        {
            //Gizmos.DrawCube(Mesh.positions[triangles[i]], Vector3.one * 0.1f);
            //Gizmos.DrawCube(Mesh.positions[triangles[i + 1]], Vector3.one * 0.1f);
            //Gizmos.DrawCube(Mesh.positions[triangles[i + 2]], Vector3.one * 0.1f);

            // Global square
            Vector2 corner = ViewCamera.WorldToViewportPoint(transform.TransformPoint(Mesh.positions[triangles[i]]));
            Vector3 a = ViewCamera.WorldToViewportPoint(transform.TransformPoint(Mesh.positions[triangles[i + 1]]))
                .ToVector2() - corner;
            Vector3 b = ViewCamera.WorldToViewportPoint(transform.TransformPoint(Mesh.positions[triangles[i + 2]]))
                .ToVector2() - corner;

            sum += Vector3.Cross(a, b).magnitude;
        }
        sum *= ViewCamera.aspect * 8f;
        return (float)sum;
    }

    float CalculateScreenSurfaceAreaFromCameraView(ProBuilderMesh mesh, IList<int> triangles)
    {
        double sum = 0.0;

        for (int i = 0; i < triangles.Count; i += 3)
        {
            // Global square
            Vector2 corner = ViewCamera.WorldToScreenPoint(transform.TransformPoint(Mesh.positions[triangles[i]]));
            Vector3 a = ViewCamera.WorldToScreenPoint(transform.TransformPoint(Mesh.positions[triangles[i + 1]]))
                .ToVector2() - corner;
            Vector3 b = ViewCamera.WorldToScreenPoint(transform.TransformPoint(Mesh.positions[triangles[i + 2]]))
                .ToVector2() - corner;

            sum += Vector3.Cross(a, b).magnitude;
        }
        sum *= 100.0f;
        return (float)sum;
    }
}
