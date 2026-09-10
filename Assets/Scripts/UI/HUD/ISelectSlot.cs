using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectSlot
{
    public GameObject Highlight { get; }

    public abstract void ToggleHighlight();
}
