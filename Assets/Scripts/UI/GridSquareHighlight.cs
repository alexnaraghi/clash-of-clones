using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

/// <summary>
/// Highlights a single grid square.
/// Adapted from SteamVR_PlayArea.
/// </summary>
public class GridSquareHighlight : MonoBehaviour
{
    public struct Quad
    {
        public Vector3 Corners0;
        public Vector3 Corners1;
        public Vector3 Corners2;
        public Vector3 Corners3;
    }

    public float borderThickness = 0.15f;
    public float wireframeHeight = 2.0f;
    public bool drawWireframeWhenSelectedOnly = false;
    public bool drawInGame = true;
    public Color color = Color.cyan;

    [HideInInspector]
    public Vector3[] vertices;

    public void UpdateBounds()
    {
        GetComponent<MeshFilter>().mesh = null; // clear existing
        BuildMesh();
    }
    
    public static bool GetBounds(float width, float height, ref Quad pRect)
    {
        try
        {
            // convert to half size in meters (from cm)
            var x = width / 2;
            var z = height / 2;

            pRect.Corners0.x = x;
            pRect.Corners0.y = 0;
            pRect.Corners0.z = z;

            pRect.Corners1.x = x;
            pRect.Corners1.y = 0;
            pRect.Corners1.z = -z;

            pRect.Corners2.x = -x;
            pRect.Corners2.y = 0;
            pRect.Corners2.z = -z;

            pRect.Corners3.x = -x;
            pRect.Corners3.y = 0;
            pRect.Corners3.z = z;

            return true;
        }
        catch { }

        return false;
    }

    public void BuildMesh()
    {
        var rect = new Quad();

        // EARLY OUT ! //
        if (!GetBounds(Consts.GridCellWidth, Consts.GridCellHeight, ref rect))
        {
            return;
        }

        var corners = new Vector3[] { rect.Corners0, rect.Corners1, rect.Corners2, rect.Corners3 };

        vertices = new Vector3[corners.Length * 2];
        for (int i = 0; i < corners.Length; i++)
        {
            var c = corners[i];
            vertices[i] = new Vector3(c.x, 0.01f, c.z);
        }

        if (borderThickness == 0.0f)
        {
            GetComponent<MeshFilter>().mesh = null;
            return;
        }

        for (int i = 0; i < corners.Length; i++)
        {
            int next = (i + 1) % corners.Length;
            int prev = (i + corners.Length - 1) % corners.Length;

            var nextSegment = (vertices[next] - vertices[i]).normalized;
            var prevSegment = (vertices[prev] - vertices[i]).normalized;

            var vert = vertices[i];
            vert += Vector3.Cross(nextSegment, Vector3.up) * borderThickness;
            vert += Vector3.Cross(prevSegment, Vector3.down) * borderThickness;

            vertices[corners.Length + i] = vert;
        }

        var triangles = new int[]
        {
            0, 4, 1,
            1, 4, 5,
            1, 5, 2,
            2, 5, 6,
            2, 6, 3,
            3, 6, 7,
            3, 7, 0,
            0, 7, 4
        };

        var uv = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 1.0f)
        };

        var colors = new Color[]
        {
            color,
            color,
            color,
            color,
            new Color(color.r, color.g, color.b, 0.0f),
            new Color(color.r, color.g, color.b, 0.0f),
            new Color(color.r, color.g, color.b, 0.0f),
            new Color(color.r, color.g, color.b, 0.0f)
        };

        var mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.colors = colors;
        mesh.triangles = triangles;

        var renderer = GetComponent<MeshRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
        renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
        renderer.lightProbeUsage = LightProbeUsage.Off;
    }

#if UNITY_EDITOR
    Hashtable values;
    void Update()
    {
        if (!Application.isPlaying)
        {
            var fields = GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            bool rebuild = false;

            if (values == null || (borderThickness != 0.0f && GetComponent<MeshFilter>().sharedMesh == null))
            {
                rebuild = true;
            }
            else
            {
                foreach (var f in fields)
                {
                    if (!values.Contains(f) || !f.GetValue(this).Equals(values[f]))
                    {
                        rebuild = true;
                        break;
                    }
                }
            }

            if (rebuild)
            {
                BuildMesh();

                values = new Hashtable();
                foreach (var f in fields)
                    values[f] = f.GetValue(this);
            }
        }
    }
#endif

    void OnDrawGizmos()
    {
        if (!drawWireframeWhenSelectedOnly)
            DrawWireframe();
    }

    void OnDrawGizmosSelected()
    {
        if (drawWireframeWhenSelectedOnly)
            DrawWireframe();
    }

    public void DrawWireframe()
    {
        if (vertices == null || vertices.Length == 0)
            return;

        var offset = transform.TransformVector(Vector3.up * wireframeHeight);
        for (int i = 0; i < 4; i++)
        {
            int next = (i + 1) % 4;

            var a = transform.TransformPoint(vertices[i]);
            var b = a + offset;
            var c = transform.TransformPoint(vertices[next]);
            var d = c + offset;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(a, c);
            Gizmos.DrawLine(b, d);
        }
    }

    // For our game we assume a constant grid size, so only generate the mesh once at runtime.
    public void Start()
    {
        if (Application.isPlaying)
        {
            GetComponent<MeshRenderer>().enabled = drawInGame;

            // No need to remain enabled at runtime.
            // Anyone that wants to change properties at runtime
            // should call BuildMesh themselves.
            enabled = false;

            // If we want the configured bounds of the user,
            // we need to wait for tracking.
            if (drawInGame)
                UpdateBounds();
        }
    }
}