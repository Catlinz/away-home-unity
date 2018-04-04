using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct UIVertexCache
{
    public List<UIVertex> vertices;
    public List<int> indices;
}

public class SideNotification : Graphic {

    public float decoHeight = 15.0f;
    public float decoWidth = 100.0f;
    public float edgeWidth = 1.0f;

    private void DrawDecoArea(ref Rect bounds, VertexHelper vh) {
        float flip = (bounds.xMax > 0) ? 1.0f : -1.0f;

        float edge_width = edgeWidth;

        // Get the vertex positions for the outer area.
        Vector2 m_tl = new Vector2(bounds.xMin, bounds.yMax);
        Vector2 m_bl = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 m_br = new Vector2(bounds.xMax, bounds.yMin);
        Vector2 m_tr = new Vector2(bounds.xMax, bounds.yMax);

        // Draw the background for the main area.
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;
        vert.color.a = 127;

        int start_idx = vh.currentVertCount;

        // +0 (Outer Top left)
        vert.position.x = m_tl.x; vert.position.y = m_tl.y;
        vh.AddVert(vert);
        // +1 (Outer Bottom Left)
        vert.position.x = m_bl.x; vert.position.y = m_bl.y;
        vh.AddVert(vert);
        // +2 (Outer Bottom Right)
        vert.position.x = m_br.x; vert.position.y = m_br.y;
        vh.AddVert(vert);
        // +3 (Outer Top Right)
        vert.position.x = m_tr.x; vert.position.y = m_tr.y;
        vh.AddVert(vert);

        // Draw the background.
        vh.AddTriangle(start_idx + 0, start_idx + 3, start_idx + 2);
        vh.AddTriangle(start_idx + 2, start_idx + 1, start_idx + 0);

        // Add the vertices for the outside of the edges.
        vert.color.a = 255;
        // +4 (Outer Top left)
        vert.position.x = m_tl.x; vert.position.y = m_tl.y;
        vh.AddVert(vert);
        // +5 (Outer Bottom Left)
        vert.position.x = m_bl.x; vert.position.y = m_bl.y;
        vh.AddVert(vert);
        // +6 (Outer Bottom Right)
        vert.position.x = m_br.x; vert.position.y = m_br.y;
        vh.AddVert(vert);
        // +7 (Outer Top Right)
        vert.position.x = m_tr.x; vert.position.y = m_tr.y;
        vh.AddVert(vert);

        // Get the vertex positions for the inside of the edges.
        float sqrt_2 = 1.0f;//Mathf.Sqrt(2.0f);
        Vector2 e_tl = new Vector2(m_tl.x + 1, m_tl.y);
        Vector2 e_bl = new Vector2(e_tl.x, m_bl.y + sqrt_2);
        Vector2 e_br = new Vector2(m_br.x - sqrt_2, e_bl.y);
        Vector2 e_tr = new Vector2(e_br.x, e_tl.y);

        // Then, Add the vertices for the inside and outside edges.
        // +8 (Inner Top Left)
        vert.position.x = e_tl.x; vert.position.y = e_tl.y;
        vh.AddVert(vert);
        // +9 (Inner Bottom Left)
        vert.position.x = e_bl.x; vert.position.y = e_bl.y;
        vh.AddVert(vert);
        // +10 (Inner Bottom Right)
        vert.position.x = e_br.x; vert.position.y = e_br.y;
        vh.AddVert(vert);
        // +11 (Inner Top Right)
        vert.position.x = e_tr.x; vert.position.y = e_tr.y;
        vh.AddVert(vert);

        // Now draw the edges.
        // LEFT
        vh.AddTriangle(start_idx + 4, start_idx + 8, start_idx + 9);
        vh.AddTriangle(start_idx + 9, start_idx + 5, start_idx + 4);
        // BOTTOM
        vh.AddTriangle(start_idx + 5, start_idx + 9, start_idx + 10);
        vh.AddTriangle(start_idx + 10, start_idx + 6, start_idx + 5);
        // RIGHT
        vh.AddTriangle(start_idx + 6, start_idx + 10, start_idx + 11);
        vh.AddTriangle(start_idx + 11, start_idx + 7, start_idx + 6);
    }

    private void DrawMainArea(ref Rect bounds, VertexHelper vh) {
        bool flip = (bounds.xMin < 0);

        float edge_width = edgeWidth;
        float radius = decoHeight;

        // Get the vertex positions for the outer area.
        Vector2 m_tl = new Vector2(bounds.xMin, bounds.yMax);
        Vector2 m_bl = new Vector2(bounds.xMin, bounds.yMin);
        Vector2 m_br = new Vector2(bounds.xMax, bounds.yMin);
        Vector2 m_tr = new Vector2(bounds.xMax, bounds.yMax);

        // Generate the list of vertices for the background
        List<UIVertex> verts = new List<UIVertex>(15);

        // Draw the background for the main area.
        UIVertex vert = UIVertex.simpleVert;
        vert.color = color;
        vert.color.a = 127;

        // V0 (Outer Top Right)
        vert.position.x = m_tr.x; vert.position.y = m_tr.y;
        verts.Add(vert);
        // V1 (Outer Top left)
        vert.position.x = m_tl.x;  vert.position.y = m_tl.y;
        verts.Add(vert);
        // (Outer Bottom left)
        if (flip) {
            // V2 (Outer Bottom Left Top)
            vert.position.x = m_bl.x; vert.position.y = m_bl.y + radius;
            verts.Add(vert);
            // V3 (Outer Bottom Left Bottom)
            vert.position.x = m_bl.x + radius; vert.position.y = m_bl.y;
            verts.Add(vert);
            // V4 (Outer Bottom Right)
            vert.position.x = m_br.x; vert.position.y = m_br.y;
            verts.Add(vert);
        }
        else {
            // V2 (Outer Bottom Left)
            vert.position.x = m_bl.x; vert.position.y = m_bl.y;
            verts.Add(vert);
            // V3 (Outer Bottom Right Bottom)
            vert.position.x = m_br.x - radius; vert.position.y = m_br.y;
            verts.Add(vert);
            // V4 (Outer Bottom Right Top)
            vert.position.x = m_br.x; vert.position.y = m_br.y + radius;
            verts.Add(vert);
        }


        vert.color.a = 255;
        vert.color.r = 0;

        // Add the outer edge vertices
        for (int i = 0; i < 5; ++i) {
            vert.position = verts[i].position;
            verts.Add(vert);
        }

        // Then, Add the vertices for the outside edges.
        // V10 (Inner Top Right)
        vert.position.x = m_tr.x - edge_width; vert.position.y = m_tr.y - edge_width;
        verts.Add(vert);
        // V11 (Inner Top Left)
        vert.position.x = m_tl.x + edge_width;  vert.position.y = m_tl.y - edge_width;
        verts.Add(vert);
        if (flip) {
            // V12 (Inner Botttom Left Top)
            vert.position.x = m_bl.x + edge_width;  vert.position.y = m_bl.y + radius;
            verts.Add(vert);
            // V13 (Inner Bottom Left Bottom)
            vert.position.x = m_bl.x + radius;  vert.position.y = m_bl.y + edge_width;
            verts.Add(vert);
            // V14 (Inner Bottom Right)
            vert.position.x = m_br.x - edge_width;  vert.position.y = m_br.y + edge_width;
            verts.Add(vert);

            vh.AddUIVertexStream(verts, new List<int> {
                0, 1, 4, 4, 1, 3, 3, 1, 2,  // Background
                10, 5, 6, 6, 11, 10, // Top
                11, 6, 7, 7, 12, 11, // Left
                7, 8, 12, 12, 8, 13, // Corner
                13, 8, 9, 9, 14, 13, // Bottom
                14, 9, 10, 10, 9, 5  // Right
            });
        }
        else {
            // V12 (Inner Botttom Left)
            vert.position.x = m_bl.x + edge_width; vert.position.y = m_bl.y + edge_width;
            verts.Add(vert);
            // V13 (Inner Bottom Right Bottom)
            vert.position.x = m_br.x - radius; vert.position.y = m_br.y + edge_width;
            verts.Add(vert);
            // V14 (Inner Bottom Right Top)
            vert.position.x = m_br.x - edge_width; vert.position.y = m_br.y + radius;
            verts.Add(vert);

            vh.AddUIVertexStream(verts, new List<int> {
                0, 1, 2, 2, 3, 0, 0, 3, 4, // Background
                10, 5, 6, 6, 11, 10, // Top
                6, 7, 11, 11, 7, 12, // Left
                12, 7, 8, 8, 12, 12, // Bottom
                13, 8, 9, 9, 14, 13, // Corner
                14, 9, 5, 5, 10, 14, // Right
            });
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
}
