using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField] private float upperBound;
    [SerializeField] private int speed;

    private void Update()
    {
        float y = Mathf.PingPong(Time.time * speed, upperBound * 2) - upperBound;
        transform.position = new Vector2(transform.position.x, y);
    }
}
