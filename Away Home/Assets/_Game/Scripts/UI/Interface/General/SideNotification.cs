using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SideNotification : Graphic {

    struct VertexCache
    {
        public UIVertex[] vertices;
        public int[] indices;
        public Rect bounds;
        public Color32 color;
        public Color32 edgeColor;
        public float edgeWidth;
        public float decoHeight;

        public bool shouldRegenVertices(ref Rect bounds, float edgeWidth, float decoHeight) {
            return (vertices == null || this.bounds != bounds || this.edgeWidth != edgeWidth || this.decoHeight != decoHeight);
        }

        public bool shouldRegenIndices(ref Rect bounds) {
            if (indices == null || this.bounds.height == 0) { return true; }
            return ((this.bounds.xMin >= 0 && bounds.xMin < 0) ||
                    (this.bounds.xMin < 0 && bounds.xMin >= 0));
        }
    }

    [Header("Content")]
    public Text title;
    public Text message;
    public Image icon;

    [Header("Style")]
    public Color edgeColor;
    public float decoHeight = 15.0f;
    public float decoWidth = 100.0f;
    public float edgeWidth = 1.0f;

    private VertexCache mainCache;
    private VertexCache decoCache;

    private float alphaInner = 1.0f;
    private float alphaOuter = 0.0f;

    private void CacheMain(ref Rect bounds) {
        CacheMainVertices(ref bounds);
        CacheMainIndices(ref bounds);
    }

    private void CacheDecoIndices(ref Rect bounds) {
        // Set the indices.
        decoCache.indices = new int[] {
            0, 1, 2, 2, 3, 0,  // Background
            9, 10, 6, 6, 5, 9, // Left
            10, 11, 6, 6, 11, 7, // Bottom
            11, 8, 4, 4, 7, 11  // Right
        };
    }

    private void CacheDecoVertices(ref Rect bounds) {
        // Make sure current cached data is reset.
        if (decoCache.vertices == null || decoCache.vertices.Length < 12) {
            decoCache.vertices = new UIVertex[12];
        }

        decoCache.color = color;
        decoCache.edgeColor = edgeColor;
        decoCache.bounds = bounds;
        decoCache.edgeWidth = edgeWidth;
        decoCache.decoHeight = decoHeight;

        bool flip = (bounds.xMin < 0);

        float edge_width = edgeWidth;

        // Get the vertex positions for the outer area.
        Vector2 m_tl = new Vector2(bounds.xMin, bounds.yMax);
        Vector2 m_bl = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 m_br = new Vector2(bounds.xMax - decoHeight, bounds.yMin);
        Vector2 m_tr = new Vector2(bounds.xMax, bounds.yMax);

        if (flip) {
            m_bl.x = bounds.xMin + decoHeight;
            m_br.x = bounds.xMax;
        }

        // Draw the background for the main area.
        UIVertex vert = UIVertex.simpleVert;
        vert.color = decoCache.color;
        vert.color.a += 50;
        vert.color.a = (byte)(vert.color.a * alphaInner);

        // V0 (Outer Top Right)
        vert.position.x = m_tr.x; vert.position.y = m_tr.y;
        decoCache.vertices[0] = vert;
        // V1 (Outer Top left)
        vert.position.x = m_tl.x; vert.position.y = m_tl.y;
        decoCache.vertices[1] = vert;
        // V2 (Outer Bottom Left)
        vert.position.x = m_bl.x; vert.position.y = m_bl.y;
        decoCache.vertices[2] = vert;
        // V3 (Outer Bottom Right)
        vert.position.x = m_br.x; vert.position.y = m_br.y;
        decoCache.vertices[3] = vert;

        // Add the vertices for the outside of the edges.
        vert.color = decoCache.edgeColor;
        vert.color.a = (byte)(vert.color.a * alphaInner);

        // Add the outer edge vertices
        for (int i = 0; i < 4; ++i) {
            vert.position = decoCache.vertices[i].position;
            decoCache.vertices[i + 4] = vert;
        }

        float ew_sqrt_2 = Mathf.Sqrt(2.0f) * edge_width;
        float inner_bl_x = edge_width / Mathf.Tan(1.178097f);

        // Then, Add the vertices for the inside and outside edges.
        // V8 (Inner Top Right)
        vert.position.x = m_tr.x - edge_width; vert.position.y = m_tr.y;
        decoCache.vertices[8] = vert;
        // V9 (Inner Top Left)
        vert.position.x = m_tl.x + ew_sqrt_2; vert.position.y = m_tl.y;
        decoCache.vertices[9] = vert;
        // V10 (Inner Bottom Left)
        vert.position.x = m_bl.x + inner_bl_x; vert.position.y = m_bl.y + edge_width;
        decoCache.vertices[10] = vert;
        // V11 (Inner Bottom Right)
        vert.position.x = m_br.x - edge_width; vert.position.y = m_br.y + edge_width;
        decoCache.vertices[11] = vert;

        for (int i = 0; i < decoCache.vertices.Length; ++i) {
            UIVertex v = decoCache.vertices[i];
            float dist = Mathf.Abs(v.position.x - decoCache.bounds.xMin) / decoCache.bounds.width;
            v.color.a = (byte)(Mathf.Lerp(alphaInner, alphaOuter, dist) * v.color.a);
            decoCache.vertices[i] = v;
        }
    }

    private void CacheMainIndices(ref Rect bounds) {
        bool flip = (bounds.xMin < 0);
        if (flip) {
            // Set the indices.
            mainCache.indices = new int[] {
                0, 1, 4, 4, 1, 3, 3, 1, 2,  // Background
                10, 5, 6, 6, 11, 10, // Top
                11, 6, 7, 7, 12, 11, // Left
                7, 8, 12, 12, 8, 13, // Corner
                13, 8, 9, 9, 14, 13, // Bottom
                14, 9, 10, 10, 9, 5  // Right
            };
        }
        else {
            mainCache.indices = new int[] {
                0, 1, 2, 2, 3, 0, 0, 3, 4, // Background
                10, 5, 6, 6, 11, 10, // Top
                6, 7, 11, 11, 7, 12, // Left
                12, 7, 8, 8, 12, 12, // Bottom
                13, 8, 9, 9, 14, 13, // Corner
                14, 9, 5, 5, 10, 14, // Right
            };
        }
    }

    private void CacheMainVertices(ref Rect bounds) {
        // Make sure current cached data is reset.
        if (mainCache.vertices == null || mainCache.vertices.Length < 15) {
            mainCache.vertices = new UIVertex[15];
        }
        mainCache.color = color;
        mainCache.edgeColor = edgeColor;
        mainCache.bounds = bounds;
        mainCache.edgeWidth = edgeWidth;
        mainCache.decoHeight = decoHeight;

        bool flip = (bounds.xMin < 0);

        float edge_width = edgeWidth;
        float radius = decoHeight*2f;

        // Get the vertex positions for the outer area.
        Vector2 m_tl = new Vector2(bounds.xMin, bounds.yMax);
        Vector2 m_bl = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 m_br = new Vector2(bounds.xMax, bounds.yMin);
        Vector2 m_tr = new Vector2(bounds.xMax, bounds.yMax);

        // Generate the list of vertices for the background

        // Draw the background for the main area.
        UIVertex vert = UIVertex.simpleVert;
        vert.color = mainCache.color;
        vert.color.a = (byte)(vert.color.a * alphaInner);

        // V0 (Outer Top Right)
        vert.position.x = m_tr.x; vert.position.y = m_tr.y;
        mainCache.vertices[0] = vert;
        // V1 (Outer Top left)
        vert.position.x = m_tl.x; vert.position.y = m_tl.y;
        mainCache.vertices[1] = vert;
        // (Outer Bottom left)
        if (flip) {
            // V2 (Outer Bottom Left Top)
            vert.position.x = m_bl.x; vert.position.y = m_bl.y + radius;
            mainCache.vertices[2] = vert;
            // V3 (Outer Bottom Left Bottom)
            vert.position.x = m_bl.x + radius; vert.position.y = m_bl.y;
            mainCache.vertices[3] = vert;
            // V4 (Outer Bottom Right)
            vert.position.x = m_br.x; vert.position.y = m_br.y;
            mainCache.vertices[4] = vert;
        }
        else {
            // V2 (Outer Bottom Left)
            vert.position.x = m_bl.x; vert.position.y = m_bl.y;
            mainCache.vertices[2] = vert;
            // V3 (Outer Bottom Right Bottom)
            vert.position.x = m_br.x - radius; vert.position.y = m_br.y;
            mainCache.vertices[3] = vert;
            // V4 (Outer Bottom Right Top)
            vert.position.x = m_br.x; vert.position.y = m_br.y + radius;
            mainCache.vertices[4] = vert;
        }


        vert.color = mainCache.edgeColor;
        vert.color.a = (byte)(vert.color.a * alphaInner);

        // Add the outer edge vertices
        for (int i = 0; i < 5; ++i) {
            vert.position = mainCache.vertices[i].position;
            mainCache.vertices[i + 5] = vert;
        }

        float ew_67_5 = edge_width / Mathf.Tan(1.178097f);

        // Then, Add the vertices for the outside edges.
        // V10 (Inner Top Right)
        vert.position.x = m_tr.x - edge_width; vert.position.y = m_tr.y - edge_width;
        mainCache.vertices[10] = vert;
        // V11 (Inner Top Left)
        vert.position.x = m_tl.x + edge_width; vert.position.y = m_tl.y - edge_width;
        mainCache.vertices[11] = vert;
        if (flip) {
            // V12 (Inner Botttom Left Top)
            vert.position.x = m_bl.x + edge_width; vert.position.y = m_bl.y + radius + ew_67_5;
            mainCache.vertices[12] = vert;
            // V13 (Inner Bottom Left Bottom)
            vert.position.x = m_bl.x + radius + ew_67_5; vert.position.y = m_bl.y + edge_width;
            mainCache.vertices[13] = vert;
            // V14 (Inner Bottom Right)
            vert.position.x = m_br.x - edge_width; vert.position.y = m_br.y + edge_width;
            mainCache.vertices[14] = vert;
        }
        else {
            // V12 (Inner Botttom Left)
            vert.position.x = m_bl.x + edge_width; vert.position.y = m_bl.y + edge_width;
            mainCache.vertices[12] = vert;
            // V13 (Inner Bottom Right Bottom)
            vert.position.x = m_br.x - radius - ew_67_5; vert.position.y = m_br.y + edge_width;
            mainCache.vertices[13] = vert;
            // V14 (Inner Bottom Right Top)
            vert.position.x = m_br.x - edge_width; vert.position.y = m_br.y + radius + ew_67_5;
            mainCache.vertices[14] = vert;
        }

        for (int i = 0; i < mainCache.vertices.Length; ++i) {
            UIVertex v = mainCache.vertices[i];
            float dist = Mathf.Abs(v.position.x - mainCache.bounds.xMin) / mainCache.bounds.width;
            v.color.a = (byte)(Mathf.Lerp(alphaInner, alphaOuter, dist) * v.color.a);
            mainCache.vertices[i] = v;
        }
    }

    private void DrawDecoArea(ref Rect bounds, VertexHelper vh) {
        if (decoCache.shouldRegenIndices(ref bounds)) {
            CacheDecoIndices(ref bounds);
        }
        if (decoCache.shouldRegenVertices(ref bounds, edgeWidth, decoHeight)) {
            CacheDecoVertices(ref bounds);
        }

        int cur_index = vh.currentVertCount;
        for (int i = 0; i < decoCache.vertices.Length; ++i) {
            vh.AddVert(decoCache.vertices[i]);
        }
        for (int i = 0; i < decoCache.indices.Length; i+=3) {
            vh.AddTriangle(cur_index + decoCache.indices[i], cur_index + decoCache.indices[i + 1], cur_index + decoCache.indices[i + 2]);
        }
    }

    private void DrawMainArea(ref Rect bounds, VertexHelper vh) {
        if (mainCache.shouldRegenIndices(ref bounds)) {
            CacheMainIndices(ref bounds);
        }
        if (mainCache.shouldRegenVertices(ref bounds, edgeWidth, decoHeight)) {
            CacheMainVertices(ref bounds);
        }

        for (int i = 0; i < mainCache.vertices.Length; ++i) {
            vh.AddVert(mainCache.vertices[i]);
        }
        for (int i = 0; i < mainCache.indices.Length; i += 3) {
            vh.AddTriangle(mainCache.indices[i], mainCache.indices[i + 1], mainCache.indices[i + 2]);
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        Debug.Log("OnPopulateMesh");

        vh.Clear();

        float deco_height = decoHeight;
        float deco_width = decoWidth;

        Rect rect = GetPixelAdjustedRect();

        Rect main_rect = new Rect(rect.xMin, rect.yMin + deco_height, rect.width, rect.height - deco_height);
        DrawMainArea(ref main_rect, vh);

        if (rect.xMin < 0) {
            // On the right side.
            Rect deco_rect = new Rect(rect.xMax - deco_width, rect.yMin, deco_width, deco_height);
            DrawDecoArea(ref deco_rect, vh);
        }
        else {
            // On the left side
            Rect deco_rect = new Rect(rect.xMin, rect.yMin, deco_width, deco_height);
            DrawDecoArea(ref deco_rect, vh);
        }
    }

    IEnumerator FadeIn() {
        for (float alpha = 0.0f; alpha < 1.0f; alpha += Time.deltaTime) {
            alphaInner = Mathf.Min(1.0f, alpha);
            decoCache.bounds.height = 0;
            mainCache.bounds.height = 0;
            SetAllDirty();
            Debug.Log("Fade in inner");
            yield return null;
        }
        for (float alpha = 0.0f; alpha < 1.0f; alpha += Time.deltaTime) {
            alphaOuter = Mathf.Min(1.0f, alpha);
            decoCache.bounds.height = 0;
            mainCache.bounds.height = 0;
            SetAllDirty();
            Debug.Log("Fade in outer");
            yield return null;
        }
    }

    protected override void OnEnable() {
        Debug.Log("ON ENABLE");
        alphaInner = 0.0f;
        alphaOuter = 0.0f;
        StartCoroutine(FadeIn());
    }
}
