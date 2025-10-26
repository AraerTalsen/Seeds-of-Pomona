using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CursorManager
{
    public static CursorType CurrentCursor { set => SetCursorSprite(value); }

    public enum CursorType
    {
        DEFAULT,
        INSPECT,
        INSPECT_GHOST,
        INTERACT,
        INTERACT_GHOST
    }

    private static Dictionary<CursorType, Texture2D> cursorSprite = new Dictionary<CursorType, Texture2D>()
    {
        {CursorType.DEFAULT, Resources.Load("Sprites/CursorDefault") as Texture2D},
        {CursorType.INSPECT, Resources.Load("Sprites/CursorInspect") as Texture2D},
        {CursorType.INSPECT_GHOST, Resources.Load("Sprites/CursorInspectGhost") as Texture2D},
        {CursorType.INTERACT, Resources.Load("Sprites/CursorInteract") as Texture2D},
        {CursorType.INTERACT_GHOST, Resources.Load("Sprites/CursorInteractGhost") as Texture2D}
    };

    private static void SetCursorSprite(CursorType type)
    {
        Cursor.SetCursor(cursorSprite[type], Vector2.zero, CursorMode.Auto);
    }
}
