using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RenderNormals : MonoBehaviour
{
    public enum Mode
    {
        PerVertex,
        PerTriangle
    }

    private MeshFilter meshFilter;
    private Mesh mesh;

    public float normalLength = 0.1f;
    public Color normalColor = Color.blue;
    public Mode NormalMode;

    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;
    }

    void OnDrawGizmosSelected()
    {
        if (NormalMode == Mode.PerVertex)
            PerVertex();
        else if (NormalMode == Mode.PerTriangle)
            PerTriangle();
    }

    void PerVertex()
    {
        if (meshFilter == null || mesh == null)
            return;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        if (vertices == null || normals == null)
            return;

        Gizmos.color = normalColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        for (int i = 0; i < mesh.vertexCount; i++)
        {
            Vector3 v = vertices[i];
            Vector3 n = normals[i];

            Gizmos.DrawLine(v, v + n * normalLength);
        }
    }

    void PerTriangle()
    {
        if (meshFilter == null || mesh == null)
            return;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        if (vertices == null || triangles == null)
            return;

        Gizmos.color = normalColor;
        Gizmos.matrix = transform.localToWorldMatrix;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v1 = vertices[triangles[i]];
            Vector3 v2 = vertices[triangles[i + 1]];
            Vector3 v3 = vertices[triangles[i + 2]];

            Vector3 n = Vector3.Cross(v2 - v1, v3 - v1).normalized;

            Vector3 center = (v1 + v2 + v3) / 3f;

            Gizmos.DrawLine(center, center + n * normalLength);
        }
    }
}