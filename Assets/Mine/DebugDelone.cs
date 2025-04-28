using System.Collections.Generic;
using UnityEngine;
namespace deloneTriangulation
{
    public class DebugDelone : MonoBehaviour
    {
        [SerializeField] List<Vector2> points = new();
        [SerializeField] List<int> triangles = new();
        [ContextMenu("triangulate points")]

        private void Update()
        {
            DrawDebugLines(points, triangles);
        }
        public void SetPoints(List<Vector2> newPoints)
        {
            points = newPoints;
            triangles = DelaunayTriangulation.Triangulate(points);
        }
        public void DrawDebugLines(List<Vector2> vertices, List<int> triangles)
        {
            #region nullcheck
            if (vertices == null || triangles == null)
            {
                return;
            }
            if (vertices.Count == 0 || triangles.Count == 0)
            {
                return;
            }
            #endregion

            for (int i = 0; i < triangles.Count; i += 3)
            {
                int indexA = triangles[i];
                int indexB = triangles[i + 1];
                int indexC = triangles[i + 2];

                Vector3 vertexA = new Vector3(vertices[indexA].x, 0f, vertices[indexA].y);
                Vector3 vertexB = new Vector3(vertices[indexB].x, 0f, vertices[indexB].y);
                Vector3 vertexC = new Vector3(vertices[indexC].x, 0f, vertices[indexC].y);

                float X = (indexC / triangles.Count) + (indexA +i / triangles.Count);
                float Y = (indexC / triangles.Count) + (indexB +i / triangles.Count);
                float Z = (indexC/triangles.Count)+ (i / triangles.Count);
                Vector3 colV = new(X,Y,Z);
                colV *= 0.5f;
                Color col = new(colV.x, colV.y, colV.z);

                Debug.DrawLine(vertexA, vertexB, col);
                Debug.DrawLine(vertexB, vertexC, col);
                Debug.DrawLine(vertexC, vertexA, col);
            }
        }
    }
}