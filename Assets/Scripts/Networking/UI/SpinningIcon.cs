using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate this object over time
/// </summary>
public class SpinningIcon : MonoBehaviour
{
    public float Speed = 200f;

    private RectTransform _rectComponent;

    private void Start()
    {
        _rectComponent = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update ()
    {
        _rectComponent.Rotate(0f, 0f, Speed * Time.deltaTime);

    }
}
