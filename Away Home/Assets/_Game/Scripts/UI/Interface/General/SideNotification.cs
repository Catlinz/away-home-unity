using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        float flip = (bounds.xMin < 0) ? -1.0f : 1.0f;

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

        // V0 (Outer Top left)
        vert.position.x = m_tl.x;  vert.position.y = m_tl.y;
        vh.AddVert(vert);
        // V1 (Outer Bottom Left)
        vert.position.x = m_bl.x;  vert.position.y = m_bl.y;
        vh.AddVert(vert);
        // V2 (Outer Bottom Right)
        vert.position.x = m_br.x;  vert.position.y = m_br.y;
        vh.AddVert(vert);
        // V3 (Outer Top Right)
        vert.position.x = m_tr.x;  vert.position.y = m_tr.y;
        vh.AddVert(vert);

        // Draw the background.
        vh.AddTriangle(0, 3, 2);
        vh.AddTriangle(2, 1, 0);

        // Add the vertices for the outside of the edges.
        vert.color.a = 255;
        // V4 (Outer Top left)
        vert.position.x = m_tl.x; vert.position.y = m_tl.y;
        vh.AddVert(vert);
        // V5 (Outer Bottom Left)
        vert.position.x = m_bl.x; vert.position.y = m_bl.y;
        vh.AddVert(vert);
        // V6 (Outer Bottom Right)
        vert.position.x = m_br.x; vert.position.y = m_br.y;
        vh.AddVert(vert);
        // V7 (Outer Top Right)
        vert.position.x = m_tr.x; vert.position.y = m_tr.y;
        vh.AddVert(vert);

        // Get the vertex positions for the inside of the edges.
        float sqrt_2 = 1.0f;//Mathf.Sqrt(2.0f);
        Vector2 e_tl = new Vector2(m_tl.x + sqrt_2, m_tl.y - sqrt_2);
        Vector2 e_bl = new Vector2(e_tl.x, m_bl.y + sqrt_2);
        Vector2 e_br = new Vector2(m_br.x - sqrt_2, e_bl.y);
        Vector2 e_tr = new Vector2(e_br.x, e_tl.y);

        // Then, Add the vertices for the inside and outside edges.
        // V8 (Inner Top Left)
        vert.position.x = e_tl.x;  vert.position.y = e_tl.y;
        vh.AddVert(vert);
        // V9 (Inner Bottom Left)
        vert.position.x = e_bl.x;  vert.position.y = e_bl.y;
        vh.AddVert(vert);
        // V10 (Inner Bottom Right)
        vert.position.x = e_br.x; vert.position.y = e_br.y;
        vh.AddVert(vert);
        // V11 (Inner Top Right)
        vert.position.x = e_tr.x; vert.position.y = e_tr.y;
        vh.AddVert(vert);

        // Now draw the edges.
        // TOP
        vh.AddTriangle(4, 7, 11);
        vh.AddTriangle(11, 8, 4);
        // LEFT
        vh.AddTriangle(4, 8, 9);
        vh.AddTriangle(9, 5, 4);
        // BOTTOM
        vh.AddTriangle(5, 9, 10);
        vh.AddTriangle(10, 6, 5);
        // RIGHT
        vh.AddTriangle(6, 10, 11);
        vh.AddTriangle(11, 7, 6);
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
