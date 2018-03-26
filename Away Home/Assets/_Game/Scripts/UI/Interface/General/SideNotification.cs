using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideNotification : Graphic {

    public float decoHeight = 15.0f;
    public float decoWidth = 100.0f;
    public float edgeWidth = 1.0f;

    private void DrawMainArea(ref Rect bounds, VertexHelper vh) {

    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        Debug.Log("OnPopulateMesh");

        float edge_width = edgeWidth;
        float deco_height = decoHeight;
        float deco_width = decoWidth;

        Rect rect = GetPixelAdjustedRect();

        float main_w = rect.width;
        float main_h = rect.height - deco_height;

        Vector2 m_tl = new Vector2(rect.xMin, rect.yMax);
        Vector2 m_bl = new Vector2(rect.xMin, rect.yMax - main_h);
        Vector2 m_br = new Vector2(rect.xMax, rect.yMax - main_h);
        Vector2 m_tr = new Vector2(rect.xMax, rect.yMax);

        // TODO: If on top or bottom, no deco rect and two tapered corners.

        // TODO: Determine where the anchor is and draw the deco rect on the correct side.
        Vector2 d_tl = new Vector2(rect.xMax - decoWidth, rect.yMax - main_h);
        Vector2 d_bl = new Vector2(d_tl.x, rect.yMin);
        Vector2 d_br = new Vector2(rect.xMax, rect.yMin);
        Vector2 d_tr = new Vector2(rect.xMax, d_tl.y);

        vh.Clear();

        // Draw the main rectangle.
        UIVertex vert = UIVertex.simpleVert;

        // V0 (Top left)
        vert.position.x = m_tl.x;
        vert.position.y = m_tl.y;
        vert.color = color;
        vh.AddVert(vert);

        // V1 (Bottom Left)
        vert.position.x = m_bl.x;
        vert.position.y = m_bl.y;
        vert.color = color;
        vh.AddVert(vert);

        // V2 (Bottom Right)
        vert.position.x = m_br.x;
        vert.position.y = m_br.y;
        vert.color = color;
        vh.AddVert(vert);

        // V3 (Top Right)
        vert.position.x = m_tr.x;
        vert.position.y = m_tr.y;
        vert.color = color;
        vh.AddVert(vert);

        vh.AddTriangle(0, 3, 2);
        vh.AddTriangle(2, 1, 0);


        // Draw the deco rectangle

        // V4 (Top left)
        vert.position.x = d_tl.x;
        vert.position.y = d_tl.y;
        vert.color = color;
        vh.AddVert(vert);

        // V5 (Bottom Left)
        vert.position.x = d_bl.x;
        vert.position.y = d_bl.y;
        vert.color = color;
        vh.AddVert(vert);

        // V6 (Bottom Right)
        vert.position.x = d_br.x;
        vert.position.y = d_br.y;
        vert.color = color;
        vh.AddVert(vert);

        // V7 (Top Right)
        vert.position.x = d_tr.x;
        vert.position.y = d_tr.y;
        vert.color = color;
        vh.AddVert(vert);

        vh.AddTriangle(4, 7, 6);
        vh.AddTriangle(6, 5, 4);

    }
}
