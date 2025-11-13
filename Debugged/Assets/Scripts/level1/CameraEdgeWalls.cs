using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEdgeWalls : MonoBehaviour
{
    [Header("Where the walls go")]
    [Range(0f, 0.25f)] public float margin = 0.05f; // % in from screen edges
    public float thickness = 1f;                    // collider thickness (Z)
    public float stickToY = 0f;                     // floor Y (overridden if bounds set)

    [Header("Auto height (optional)")]
    public Collider corridorBounds;                 // your CameraBounds BoxCollider/volume
    public float extraTop = 1f;                     // add a little headroom

    [Header("Physics")]
    public LayerMask wallLayer;                     // set to your "wall" layer

    // runtime objects
    Camera cam;
    BoxCollider leftCol, rightCol;

    float Height
    {
        get
        {
            if (corridorBounds)
            {
                var b = corridorBounds.bounds;
                // floor Y
                stickToY = b.min.y;
                return (b.size.y + extraTop);
            }
            // fallback if no bounds provided
            return 8f;
        }
    }

    void Awake()
    {
        cam = GetComponent<Camera>();
        leftCol  = CreateWall("LeftEdgeWall");
        rightCol = CreateWall("RightEdgeWall");
    }

    BoxCollider CreateWall(string name)
    {
        var go = new GameObject(name);
        go.transform.parent = transform;
        go.layer = LayerMaskToLayer(wallLayer);
        var bc = go.AddComponent<BoxCollider>();
        bc.isTrigger = false; // solid walls
        return bc;
    }

    int LayerMaskToLayer(LayerMask m)
    {
        int mask = m.value;
        for (int i = 0; i < 32; i++) if ((mask & (1 << i)) != 0) return i;
        return 0; // Default if none set
    }

   // --- replace your LateUpdate() with this ---
void LateUpdate()
{
    float h = Height;

    // Viewport edge points
    Vector3 vLeft  = new Vector3(margin,      0.5f, cam.nearClipPlane + 10f);
    Vector3 vRight = new Vector3(1f - margin, 0.5f, cam.nearClipPlane + 10f);

    // Convert to world
    Vector3 wL = cam.ViewportToWorldPoint(vLeft);
    Vector3 wR = cam.ViewportToWorldPoint(vRight);

    // Use corridor bounds to get correct floor Y and gameplay Z
    float zPlane = corridorBounds ? corridorBounds.bounds.center.z : 0f;
    float yFloor = stickToY; // stickToY is already set by Height() when bounds exist

    // Positions
    leftCol.transform.position  = new Vector3(wL.x, yFloor + h * 0.5f, zPlane);
    rightCol.transform.position = new Vector3(wR.x, yFloor + h * 0.5f, zPlane);

    // Sizes: thin on X, full corridor height, corridor depth on Z
    float zSize = corridorBounds ? Mathf.Max(thickness, corridorBounds.bounds.size.z) : thickness;
    leftCol.size  = new Vector3(0.1f, h, zSize);
    rightCol.size = new Vector3(0.1f, h, zSize);
}


    void OnDrawGizmos()
    {
        if (!cam) cam = GetComponent<Camera>();
        float h = Application.isPlaying ? Height : (corridorBounds ? corridorBounds.bounds.size.y + extraTop : 8f);

        // gizmo color
        Color c = new Color(1f, 0.92f, 0.2f, 0.9f);
        Gizmos.color = c;

        // draw two vertical gizmo planes at the edges
        Vector3 vL = cam.ViewportToWorldPoint(new Vector3(margin,      0.5f, cam.nearClipPlane + 10f));
        Vector3 vR = cam.ViewportToWorldPoint(new Vector3(1f - margin, 0.5f, cam.nearClipPlane + 10f));
        Vector3 basePos = transform.position;

        Vector3 lPos = new Vector3(vL.x, stickToY + h * 0.5f, basePos.z);
        Vector3 rPos = new Vector3(vR.x, stickToY + h * 0.5f, basePos.z);
        Vector3 size = new Vector3(0.05f, h, thickness);

        Gizmos.DrawWireCube(lPos, size);
        Gizmos.DrawWireCube(rPos, size);
    }
}
