using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.PodSquad.GDPPNF
{
    /// <summary>
    /// Loads the proper player preff into this input key
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class RoomNameInputField : MonoBehaviour
    {
        static string roomNamePrefKey = "RoomName";

        void Start()
        {
            string defaultName = "";
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(roomNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(roomNamePrefKey);
                    _inputField.text = defaultName;
                }
            }
        }
    }
}