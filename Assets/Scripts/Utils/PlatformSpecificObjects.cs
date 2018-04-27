
using UnityEngine;


/// <summary>
/// A script that will allow the simple activation of the objects that 
/// it is on depending on the platform.
/// Author: Ben Hoffman
/// </summary>
public class PlatformSpecificObjects : MonoBehaviour
{
    public GameObject[] MobileObjects;
    public GameObject[] DesktopObjects;

    private void Awake()
    {
        _init();    
    }

    private void _init()
    {
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_STANDALONE_WIN

        foreach(GameObject g in DesktopObjects)
        {
            g.SetActive(true);
        }
        foreach (GameObject g in MobileObjects)
        {
            g.SetActive(false);
        }

#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
        foreach(GameObject g in DesktopObjects)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in MobileObjects)
        {
            g.SetActive(true);
        }
#endif
    }


}
