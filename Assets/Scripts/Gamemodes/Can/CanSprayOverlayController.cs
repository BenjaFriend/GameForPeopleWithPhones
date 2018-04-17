using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSprayOverlayController : MonoBehaviour
{
    private void Start()
    {
        CanController.Instance.OnCanBrokenEvent += activateSelf;
        gameObject.SetActive(false); // deactivate self
    }

    private void activateSelf()
    {
        gameObject.SetActive(true);
        CanController.Instance.OnCanBrokenEvent -= activateSelf;
    }
}
