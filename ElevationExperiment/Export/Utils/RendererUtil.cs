using System;
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

    public static T[] AddRange<T>(this T[] array, IEnumerable<T> elements)
    {
        List<T> list = array.ToList();
        list.AddRange(elements);
        return list.ToArray();
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

    public static void Apply(this Mesh mesh, Quad quad)
    {
        mesh.vertices = quad.vertices;
        mesh.triangles = quad.triangles;
        mesh.normals = quad.normals;
        mesh.uv = quad.uvs;
    }

    public static void Add(this Mesh mesh, Quad quad)
    {
        mesh.vertices = mesh.vertices.AddRange(quad.vertices);
        mesh.triangles = mesh.triangles.AddRange(quad.triangles);
        mesh.normals = mesh.normals.AddRange(quad.normals);
        mesh.uv = mesh.uv.AddRange(quad.uvs);
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

public struct Quad
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Vector2[] uvs;
}

