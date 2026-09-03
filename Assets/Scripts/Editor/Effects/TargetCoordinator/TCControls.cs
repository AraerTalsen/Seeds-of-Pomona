using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TCControls
{
    public static void MouseControls(Rect gridOrigin, Vector2 prevPos, out Vector2 position)
    {
        Event e = Event.current;
        position = prevPos;
        int controlId = GUIUtility.GetControlID(FocusType.Passive);

        switch (e.type)
        {
            case EventType.MouseDown:
                if (gridOrigin.Contains(e.mousePosition))
                {
                    position = e.mousePosition;
                    GUIUtility.hotControl = controlId;
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlId)
                {
                    position = e.mousePosition;
                    e.Use();
                    UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
                }
                break;

            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlId)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
        }
    }
}
