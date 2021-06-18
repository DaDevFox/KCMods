﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ArrayExtensions
{
    public static T[] Add<T>(this T[] array, T element)
    {
        List<T> list = array.ToList();
        list.Add(element);
        return list.ToArray();
    }

    public static T[] AddRange<T>(this T[] array, IEnumerable<T> collection)
    {
        int count = collection.Count();
        T[] result = new T[array.Length + count];
        array.CopyTo(result, 0);
        for (int i = array.Length; i < result.Length; i++)
        {
            result[i] = collection.ElementAt(i - array.Length);
        }

        return result;

        // List<T> list = array.ToList();
        // list.AddRange(collection);
        // return list.ToArray();
    }

    public static T[] AddRange<T>(this T[] array, T[] other)
    {
        T[] result = new T[array.Length + other.Length];
        array.CopyTo(result, 0);
        for (int i = array.Length; i < result.Length; i++)
        {
            result[i] = other[i - array.Length];
        }

        return result;
    }

    /// <summary>
    /// Sets the last [<c>other.Length</c>] elements of the array <c>array</C> to other's elements. The completed array will always be of length <c>length</c>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="other"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static T[] SetLast<T>(this T[] array, T[] other, int length)
    {
        T[] result = new T[length];
        for (int i = 0; i < result.Length; i++)
            if (i < array.Length)
                result[i] = array[i];

        for (int i = result.Length - 1; i >= result.Length - other.Length; i--)
        {
            result[i] = other[result.Length - 1 - i];
        }

        return result;
    }
}

public static class RendererUtil
{
    /// <summary>
    /// Creates a quad's data
    /// </summary>
    /// <param name="dimensions">dimensions of the quad</param>
    /// <param name="position">position of the quad</param>
    /// <param name="masterSize">num of already added vertices in the master mesh if this is being included in a master mesh (offsets indices)</param>
    /// <returns></returns>
    public static Quad Quad(Vector2 dimensions, Vector3 position, int masterSize = 0)
    {
        float width = dimensions.x;
        float height = dimensions.y;
        float x = position.x;
        float y = position.y;
        float z = position.z;

        return new Quad()
        {
            vertices = new Vector3[]
            {
                    new Vector3(x, y, z) + new Vector3(0, 0, 0),
                    new Vector3(x, y, z) + new Vector3(width, 0, 0),
                    new Vector3(x, y, z) + new Vector3(0, height, 0),
                    new Vector3(x, y, z) + new Vector3(width, height, 0)
            },
            triangles = new int[]
            {
                // lower left triangle
                masterSize + 0, masterSize + 2, masterSize + 1,
                // upper right triangle
                masterSize + 2, masterSize + 3, masterSize + 1
            },
            normals = new Vector3[]
            {
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward
            },
            uvs = new Vector2[]
            {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
            }
        };
    }

    /// <summary>
    /// Simple depth 1 subdivision of a traditional quad; hardcoded for speed
    /// </summary>
    /// <param name="dimensions"></param>
    /// <param name="position"></param>
    /// <param name="masterSize"></param>
    /// <returns></returns>
    public static Quad NinePointQuad(Vector2 dimensions, Vector3 position, int masterSize = 0)
    {
        float width = dimensions.x;
        float height = dimensions.y;
        float x = position.x;
        float y = position.y;
        float z = position.z;

        return new Quad()
        {
            vertices = new Vector3[]
            {
                    new Vector3(x, y, z) + new Vector3(0, 0, 0),
                    new Vector3(x, y, z) + new Vector3(width/2, 0, 0),
                    new Vector3(x, y, z) + new Vector3(0, height/2, 0),
                    new Vector3(x, y, z) + new Vector3(width/2, height/2, 0),
                    new Vector3(x, y, z) + new Vector3(width, 0, 0),
                    new Vector3(x, y, z) + new Vector3(width, height/2, 0),
                    new Vector3(x, y, z) + new Vector3(width, height, 0),
                    new Vector3(x, y, z) + new Vector3(width/2, height, 0),
                    new Vector3(x, y, z) + new Vector3(0, height, 0),
            },
            triangles = new int[]
            {
                    // 0, 0 left triangle
                    masterSize + 0, masterSize + 2, masterSize + 1,
                    // 0, 0 right triangle
                    masterSize + 2, masterSize + 3, masterSize + 1,

                    // 0.5, 0.5 left
                    masterSize + 1, masterSize + 3, masterSize + 4,
                    // 0.5, 0.5 right
                    masterSize + 3, masterSize + 5, masterSize + 4,
                    
                    // 1, 0.5 left
                    masterSize + 2, masterSize + 8, masterSize + 3,
                    // 1, 0.5 right
                    masterSize + 8, masterSize + 7, masterSize + 3,
                    
                    // 0.5, 1 left
                    masterSize + 3, masterSize + 7, masterSize + 5,
                    // 0.5, 1 right
                    masterSize + 7, masterSize + 6, masterSize + 5,
            },
            normals = new Vector3[]
            {
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward,
                    -Vector3.forward
            },
            uvs = new Vector2[]
            {
                    new Vector2(0f, 0f),
                    new Vector2(0.5f, 0f),
                    new Vector2(0f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(1f, 0f),
                    new Vector2(1f, 0.5f),
                    new Vector2(1f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, 1f)
            }
        };
    }

    /// <summary>
    /// Applies a quad to a mesh
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="quad"></param>
    public static void Apply(this Mesh mesh, Quad quad)
    {
        mesh.vertices = quad.vertices;
        mesh.triangles = quad.triangles;
        mesh.normals = quad.normals;
        mesh.uv = quad.uvs;
    }

    /// <summary>
    /// Adds a quad to a mesh vertices, tris, normals, and uvs
    /// </summary>
    /// <param name="mesh"></param>
    /// <param name="quad"></param>
    public static void Add(this Mesh mesh, Quad quad)
    {
        mesh.vertices = mesh.vertices.AddRange(quad.vertices);
        mesh.triangles = mesh.triangles.AddRange(quad.triangles);

        mesh.normals = mesh.normals.SetLast(quad.normals, mesh.vertices.Length);
        mesh.uv = mesh.uv.SetLast(quad.uvs, mesh.vertices.Length);
    }

    /// <summary>
    /// Modifies a quad to rotate it towards the given normal vector
    /// </summary>
    /// <param name="quad">subject quad</param>
    /// <param name="normal">direction to face in a normal vector</param>
    /// <param name="pivot">local pivot</param>
    public static Quad SetRotation(this Quad quad, Vector3 normal, Vector3 pivot, Vector3 up)
    {
        Quaternion newRotation = Quaternion.LookRotation(-normal, up);

        for (int i = 0; i < quad.vertices.Length; i++)
        {
            quad.vertices[i] = newRotation * (quad.vertices[i] - pivot) + pivot;
        }

        return quad;
    }
}

/// <summary>
/// Generic mesh struct for a grouping of points, triangles, normals, and uvs
/// </summary>
public struct Quad
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uvs;
}
