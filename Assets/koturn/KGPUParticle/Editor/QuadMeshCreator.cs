using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;


namespace Koturn.KGPUParticle
{
    /// <summary>
    /// Quad Mesh Creator.
    /// </summary>
    public static class QuadMeshCreator
    {
        /// <summary>
        /// Apply mesh to the <<see cref="GameObject"/>.
        /// </summary>
        /// <param name="go">Target <<see cref="GameObject"/>.</param>
        /// <param name="mesh">A mesh to set.</param>
        public static void ApplyMesh(GameObject go, Mesh mesh)
        {
            // Add MeshFilter if not exists.
            var meshFilter = go.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = go.AddComponent<MeshFilter>();
            }
            meshFilter.sharedMesh = mesh;

            // Add MeshRenderer if not exists.
            if (go.GetComponent<MeshRenderer>() == null)
            {
                var meshRenderer = go.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
            }
        }

        /// <summary>
        /// Resize box collider of the <<see cref="GameObject"/>.
        /// </summary>
        /// <param name="go">Target <<see cref="GameObject"/>.</param>
        /// <param name="size">New size of box collider.</param>
        public static void ResizeBoxCollider(GameObject go, Vector3 size)
        {
            // Resize box collider if exists.
            var boxCollider = go.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.size = size;
            }
        }

        /// <summary>
        /// <para>Create cube mesh with 8 vertices, 12 polygons (triangles) and no UV coordinates.</para>
        /// <para>UVs and normals of created Quad are incorrect.</para>
        /// </summary>
        /// <param name="size">Size of cube.</param>
        /// <param name="div">Number of divisions for X and Y.</param>
        /// <param name="hasUV">A flag whether adding UV coordinate to mesh or not.</param>
        /// <param name="hasNormal">A flag whether adding Normal coordinate to mesh or not.</param>
        /// <param name="hasTangent">A flag whether adding Tangent coordinate to mesh or not.</param>
        /// <param name="hasVertexColor">A flag whether adding color to mesh or not.</param>
        /// <returns>Created cube Mesh.</returns>
        public static Mesh CreateQuadMesh(Vector2 size, Vector2Int div, bool hasUV = false, bool hasNormal = false, bool hasTangent = false, bool hasVertexColor = false)
        {
            var mesh = new Mesh();

            var bottomLeftPos = new Vector2(-size.x * 0.5f, -size.y * 0.5f);
            var topRightPos = new Vector2(size.x * 0.5f, size.y * 0.5f);
            var offset = new Vector2(size.x / div.x, size.y / div.y);

            var vertices = new Vector3[(div.y + 1) * (div.x + 1)];
            var strideX = div.x + 1;
            for (int i = 0; i < div.y; i++)
            {
                for (int j = 0; j < div.x; j++)
                {
                    vertices[i * strideX + j] = new Vector3(bottomLeftPos.x + offset.x * j, bottomLeftPos.y + offset.y * i, 0.0f);
                }
                vertices[i * strideX + div.x] = new Vector3(topRightPos.x, bottomLeftPos.y + offset.y * i, 0.0f);
            }
            for (int j = 0; j < div.x; j++)
            {
                vertices[div.y * strideX + j] = new Vector3(bottomLeftPos.x + offset.x * j, topRightPos.y, 0.0f);
            }
            vertices[div.y * strideX + div.x] = new Vector3(topRightPos.x, topRightPos.y, 0.0f);
            mesh.SetVertices(vertices);

            var triangles = new int[3 * 2 * div.x * div.y];
            var triIdx = 0;
            var offsetX = size.x / div.x;
            var offsetY = size.y / div.y;
            for (int i = 0; i < div.y; i++)
            {
                for (int j = 0; j < div.x; j++)
                {
                    // (j, i), (j, i + 1), (j + 1, i + 1)
                    triangles[triIdx++] = i * strideX + j;
                    triangles[triIdx++] = (i + 1) * strideX + j;
                    triangles[triIdx++] = (i + 1) * strideX + (j + 1);
                    // (j, i), (j + 1, i + 1), (j, i + 1)
                    triangles[triIdx++] = i * strideX + j;
                    triangles[triIdx++] = (i + 1) * strideX + (j + 1);
                    triangles[triIdx++] = i * strideX + (j + 1);
                }
            }
            mesh.SetTriangles(triangles, 0);

            if (hasUV || hasVertexColor)
            {
                // 3:(0,1)  4:(1,1)
                //
                //
                // 1:(0,0)  2:(1,0)
                var uv = new Vector2[vertices.Length];
                var uvOffset = new Vector2(1.0f / div.x, 1.0f / div.y);
                for (int i = 0; i < div.y; i++)
                {
                    for (int j = 0; j < div.x; j++)
                    {
                        uv[i * strideX + j] = new Vector2(uvOffset.x * j, uvOffset.y * i);
                    }
                    uv[i * strideX + div.x] = new Vector2(1.0f, uvOffset.y * i);
                }
                for (int j = 0; j <= div.x; j++)
                {
                    uv[div.y * strideX + j] = new Vector2(uvOffset.x * j, 1.0f);
                }
                uv[div.y * strideX + div.x] = new Vector2(1.0f, 1.0f);

                if (hasUV)
                {
                    mesh.SetUVs(0, uv);
                }

                if (hasVertexColor)
                {
                    mesh.SetColors(CreateColorsFromUV(uv));
                }
            }

            mesh.Optimize();
            mesh.RecalculateBounds();
            if (hasNormal)
            {
                mesh.RecalculateNormals();
            }
            if (hasTangent)
            {
                mesh.RecalculateTangents();
            }

            return mesh;
        }

        /// <summary>
        /// Convert UV coordinates to RGB colors.
        /// </summary>
        /// <param name="uv">UV coordinates.</param>
        /// <returns><see cref="Color"/> array created from <paramref name="vertexData"/>.</returns>
        private static Color[] CreateColorsFromUV(Vector2[] uv, float colorB = 1.0f)
        {
            var colors = new Color[uv.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(uv[i].x, uv[i].y, colorB);
            }

            return colors;
        }
    }
}
