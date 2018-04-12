using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.PodSquad.GDPPNF
{
    public class RoomInfoPanel : MonoBehaviour
    {
        [Tooltip("The text element to display the room name")]
        [SerializeField]
        private Text _roomNameText;
        [Tooltip("The text element to display the number of players")]
        [SerializeField]
        private Text _numPlayersText;
        [Tooltip("The button that will tell the game to try and join this room")]
        [SerializeField]
        private Button _joinRoombutton;

        public Text RoomNameText
        {
            get
            {
                return _roomNameText;
            }

            set
            {
                _roomNameText = value;
            }
        }

        public Text NumPlayersText
        {
            get
            {
                return _numPlayersText;
            }

            set
            {
                _numPlayersText = value;
            }
        }

        public Button JoinRoombutton
        {
            get
            {
                return _joinRoombutton;
            }

            set
            {
                _joinRoombutton = value;
            }
        }
    }
}