using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCanShakeRaiseEvent : MonoBehaviour
{
	public void SendTestEvent()
    {
        Debug.Log("[TestCanShakeRaiseEvent] Test can shake event!");

        byte evCode = 0;    // my event 0. could be used as "group units"
        byte[] content = new byte[] { 1, 2, 5, 10 };    // e.g. selected unity 1,2,5 and 10
        bool reliable = true;
        PhotonNetwork.RaiseEvent(
            evCode,
            content,
            reliable,
            new RaiseEventOptions() { Receivers = ReceiverGroup.All
            }
       );
    }
}
