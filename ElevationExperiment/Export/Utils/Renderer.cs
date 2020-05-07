using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Zat.Shared.Rendering;

namespace ElevationExperiment.Utils
{
    public class FoxRenderer
    {
        public static void DrawZatLine(Camera cam, Vector3[] points, float width, Color color)
        {
            bool firstPoint = true;
            Vector3 lastPoint = Vector3.zero;
            foreach(Vector3 point in points)
            {
                if (!firstPoint)
                {
                    Vector3 a = cam.WorldToScreenPoint(lastPoint);
                    Vector3 b = cam.WorldToScreenPoint(point);

                    Vector2 a2D = new Vector2(a.x, a.y);
                    Vector2 b2D = new Vector2(a.x, a.y);

                    ZatsRenderer.DrawLine(a2D, b2D, width, color);

                    firstPoint = true;
                }

                lastPoint = point;
            }
        }

        public static void DrawZatLine(Camera cam, Vector3 start, Vector3 end, float width, Color color)
        {
            Vector3[] points = new Vector3[2];

            points[0] = start;
            points[1] = end;

            DrawZatLine(cam, points, width, color);
        }



        public static void DrawLine(Vector3 start, Vector3 end, float width = 0.1f, float duration = 10f, Color color = new Color())
        {
            Vector3[] vertices = new Vector3[2];

            vertices[0] = start;
            vertices[1] = end;

            GameObject GO = GameObject.Instantiate(new GameObject());
            Line l = GO.AddComponent<Line>();
            l.Init(vertices, width, duration, color);
        }

        public static void DrawLine(Vector3[] vertices, float width = 0.1f, float duration = 10f, Color color = new Color())
        {
            GameObject GO = GameObject.Instantiate(new GameObject());
            Line l = GO.AddComponent<Line>();
            l.Init(vertices, width, duration, color);
        }

        public class Line : MonoBehaviour
        {
            private Vector3[] points;

            private float m_width;
            private float m_duration;
            private float m_timeAlive = 0f;
            private Color m_color;

            private Material material;



            // Got help (pretty much copied) from: 
            // https://docs.unity3d.com/ScriptReference/GL.html
            public void Init(Vector3 start, Vector3 end, float width = 0.1f, float duration = 10f, Color color = new Color())
            {
                points = new Vector3[2];
                points[0] = start;
                points[1] = end;

                m_width = width;
                m_duration = duration;

                m_color = color;

                Create();
            }

            public void Init(Vector3[] vertices, float width = 0.1f, float duration = 10f, Color color = new Color())
            {
                points = vertices;

                m_width = width;
                m_duration = duration;

                m_color = color;

                Create();
            }

            void Update()
            {
                if(m_timeAlive > m_duration)
                {
                    GameObject.Destroy(gameObject);
                }

                m_timeAlive += Time.deltaTime;
            }

            void Create()
            {
                SetupMaterial();
                material.SetPass(0);


                GL.PushMatrix();
                GL.MultMatrix(transform.localToWorldMatrix);

                GL.Begin(GL.LINES);

                foreach (Vector3 point in points)
                {
                    GL.Vertex(point);
                }

                GL.End();
                GL.PopMatrix();
            }

            void SetupMaterial()
            {
                if (!material)
                {
                    Shader shader = Shader.Find("Hidden/Internal-Colored");
                    material = new Material(shader);

                    material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);

                    material.SetInt("_Cull", (int)CullMode.Off);
                    material.SetInt("_ZWrite", 0);

                    material.color = m_color;
                }
            }

        }

    }
}
