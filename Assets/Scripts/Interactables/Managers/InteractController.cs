using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractController : MonoBehaviour
{
    public virtual bool IsInInteractSpace { get; }

    public virtual Interactable CurrentPriority { get; set; }

    public abstract void AddInteraction(Interactable interact);

    public abstract void RemoveInteraction(Interactable interact);

    public abstract void ClearInteractions();

    public abstract void Interact();
}
