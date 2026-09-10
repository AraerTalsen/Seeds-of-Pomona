using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectSlot : MonoBehaviour, ISelectSlot
{
    [SerializeField] private GameObject highlight;
    [SerializeField] private Image item;
    [SerializeField] private Slider coolDownProgress;

    public GameObject Highlight => highlight;
    public Slider CoolDownProgress { get => coolDownProgress; set => coolDownProgress = value; }

    public void ToggleHighlight() => highlight.SetActive(!highlight.activeSelf);
    public void SetSprite(Sprite sprite) => item.sprite = sprite;
}
