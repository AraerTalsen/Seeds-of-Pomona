using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TCPainter
{
    private static Rect panel;
    public static float PanelHeight => EditorGUIUtility.singleLineHeight * 25;

    public static Rect ClampRectToPanel(Rect bounds, Rect unit)
    {
        float x = Mathf.Clamp(unit.x, bounds.xMin, bounds.xMax - unit.width);
        float y = Mathf.Clamp(unit.y, bounds.yMin, bounds.yMax - unit.height);
        return new Rect(x, y, unit.width, unit.height);
    }

    public static Vector2 CenterOnUnit(Sprite unit, Vector2 point)
    {
        float x = point.x - unit.texture.width / 2;
        float y = point.y - unit.texture.height / 2;

        return new Vector2(x, y);
    }

    public static Sprite GenerateSprite()
    {
        int width = 32;
        int height = 32;
        Texture2D tex = new(width, height);

        Color32 green = new Color(0.56f, 1, 0.56f, 1);
        Color32[] cols = new Color32[width * height];
        System.Array.Fill(cols, green);

        tex.SetPixels32(cols);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }
}
