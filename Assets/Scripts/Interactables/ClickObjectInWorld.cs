using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

//Mouse event handling to detect if an IClickable has been hovered over or clicked on
public class ClickObjectInWorld : MonoBehaviour
{
    private IClickable interactable;
    private bool justEntered = false;

    public bool PauseWorldClicks { get; set; }

    void Update()
    {
        if (!PauseWorldClicks)
        {
            PointerCleanUp();
            CheckForClickables();
        }
    }

    //Clean up references to ClickableObjects on units that have been destroyed
    private void PointerCleanUp()
    {
        if (interactable != null && interactable.Equals(null))
        {
            interactable = null;
        }
    }

    private void CheckForClickables()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        if (hits.Length > 0)
        {
            IClickable temp = FindFirstInteractable(hits);
            if (temp != interactable)
            {
                interactable?.OnHoverExit(gameObject);
                justEntered = temp != null;
                interactable = temp;
            }
            CheckForInteraction();
        }
        else if (interactable != null)
        {
            interactable.OnHoverExit(gameObject);
            interactable = null;
            justEntered = false;
        }
    }

    private IClickable FindFirstInteractable(RaycastHit2D[] hits)
    {
        return hits.Select(hit => hit.transform.gameObject.GetComponent<IClickable>()).FirstOrDefault(interact => interact != null);
    }

    private void CheckForInteraction()
    {
        if (interactable != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                interactable.OnClick(gameObject);
            }
            else if (justEntered)
            {
                justEntered = false;
                interactable.OnHoverEnter(gameObject);
            }
            else
            {
                interactable.OnHover(gameObject);
            }
        }
    }
}
