
using UnityEngine;

/// <summary>
/// Script that will set the countdown text on the countdown tick event
/// </summary>
public class CountdownText : Photon.PunBehaviour
{
    [Space(10)]
    [Header("UI")]
    public UnityEngine.UI.Text CountdownTextUI;

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += this._onEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= this._onEvent;
    }

    private void _onEvent(byte eventcode, object content, int senderid)
    {
        if (CountdownTextUI == null) return;

        object[] selected = content as object[];


        if (eventcode == (byte)Constants.EVENT_ID.START_COUNTDOWN_FINISHED)
        {
            // TODO: Set the countdown UI to nothing
            CountdownTextUI.text = "";
        }
        else if (eventcode == (byte)Constants.EVENT_ID.COUNTDOWN_TICK)
        {
            byte num = (byte)selected[0];

            CountdownTextUI.text = num.ToString();
        }
    }
}
