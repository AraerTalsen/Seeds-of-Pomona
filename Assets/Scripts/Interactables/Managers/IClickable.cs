using UnityEngine;

public interface IClickable
{
    public float Range { get; }

    public abstract void OnHoverEnter(GameObject player);
    public abstract void OnHover(GameObject player);
    public abstract void OnHoverExit(GameObject player);
    public abstract void OnClick(GameObject player);
}
