using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;
using Math = UnityEngine.ProBuilder.Math;
using Random = UnityEngine.Random;

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

    void Start()
    {

    }

    [Button()]
    void AddFace()
    {
        var faces = Mesh.GetSelectedFaces();
        if(faces == null)
            return;
        if (faces.Length != 1)
        {
            Debug.Log("Should be one face selected for Adding");
            return;
        }

        if (FacesSetup == null)
            FacesSetup = new List<FaceSetup>();
        FacesSetup.Add(new FaceSetup{Face = faces[0]});
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
            Debug.Log("Should be one face selected for Adding");
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

    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        foreach (var faceSetup in FacesSetup)
        {
            // Draw normal
            //faceSetup.Face.indexes
            //Gizmos.color = Color.magenta;
            //Gizmos.DrawLine(Vector3.zero, Vector3.one * 10);

            // Calculate area

            // Calculate screen space area


        }
            
        

    }
}
