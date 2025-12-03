using UnityEngine;

public class Inspectable : MonoBehaviour, IClickable
{
    [SerializeField]
    private float range;
    [SerializeField]
    protected string message;
    [SerializeField]
    private bool isInspectable = true;

    public float Range => range;
    public virtual bool IsInspectable { get => isInspectable; set => isInspectable = value; }

    public void OnClick(GameObject player)
    {
        if (isInspectable && IsInRange(player))
        {
            OnHoverExit(player);
            TextWindowManager.Instance.SetMessage(message, player);
        }
    }

    public void OnHoverEnter(GameObject player)
    {
        if (isInspectable)
        {
            CursorManager.CurrentCursor = IsInRange(player) ? CursorManager.CursorType.INSPECT : CursorManager.CursorType.INSPECT_GHOST;
        }
    }

    public void OnHover(GameObject player)
    {
        if (isInspectable)
        {
            CursorManager.CurrentCursor = IsInRange(player) ? CursorManager.CursorType.INSPECT : CursorManager.CursorType.INSPECT_GHOST;
        }
    }

    public void OnHoverExit(GameObject player)
    {
        if (isInspectable)
        {
            CursorManager.CurrentCursor = CursorManager.CursorType.DEFAULT;
        }
    }
    
    private bool IsInRange(GameObject interactor)
    {
        return Range >= Vector3.Distance(transform.position, interactor.transform.position);
    }
}
