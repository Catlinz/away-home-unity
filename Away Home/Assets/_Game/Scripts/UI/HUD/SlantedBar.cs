using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlantedBar : Graphic {
	protected override void OnPopulateMesh(VertexHelper vh)
    {
		Debug.Log("OnPopulateMesh");

		float width = GetPixelAdjustedRect().width;
		float height = GetPixelAdjustedRect().height;

		float fH = (2.0f * height) / 15.0f;
		float barH = (0.5f * height);

		float cos675 = Mathf.Cos(67.5f * Mathf.Deg2Rad);
		float cos45 = Mathf.Cos(45.0f * Mathf.Deg2Rad);
		float sin45 = Mathf.Sin(45.0f * Mathf.Deg2Rad);

		Vector2 p0 = new Vector2(width - fH, barH + fH);
		Vector2 p1 = new Vector2(barH - cos675*fH, p0.y);
		Vector2 p2 = new Vector2(p1.x + sin45 * barH, p1.y + sin45 * barH);

		float size = 1.0f;

		vh.Clear();

		UIVertex vert = UIVertex.simpleVert;
		
		// V0
		vert.position.x = p0.x;
		vert.position.y = p0.y - (0.5f * size);
		vert.color = color;
		vh.AddVert(vert);

		float cosA = Mathf.Cos(67.5f * Mathf.PI / 180.0f);
		float sinA = Mathf.Sin(67.5f * Mathf.PI / 180.0f);

		// V1
		vert.position.x = p1.x + cosA*0.5f * size;
		vert.position.y = p1.y + sinA * 0.5f * size;
		vert.color = color;
		vh.AddVert(vert);

		// V2
		vert.position.x = p2.x + Mathf.Sin(Mathf.PI*0.25f)*0.5f * size;
		vert.position.y = p2.y + Mathf.Sin(Mathf.PI*0.25f)*0.5f * size;
		vert.color = color;
		vh.AddVert(vert);

		// V3
		vert.position.x = p2.x - Mathf.Sin(Mathf.PI*0.25f)*0.5f * size;
		vert.position.y = p2.y - Mathf.Sin(Mathf.PI*0.25f)*0.5f * size;
		vert.color = color;
		vh.AddVert(vert);

		// V4
		vert.position.x = p1.x - cosA*0.5f * size;
		vert.position.y = p1.y - sinA * 0.5f * size;
		vert.color = color;
		vh.AddVert(vert);

		// V0
		vert.position.x = p0.x;
		vert.position.y = p0.y + (0.5f * size);
		vert.color = color;
		vh.AddVert(vert);

        vh.AddTriangle(0, 5, 1);
        vh.AddTriangle(4, 1, 0);

		vh.AddTriangle(4, 3, 1);
		vh.AddTriangle(1, 3, 2);
    }
}
