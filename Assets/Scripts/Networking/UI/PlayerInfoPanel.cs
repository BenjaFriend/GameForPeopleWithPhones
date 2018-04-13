using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.PodSquad.GDPPNF
{
    public class PlayerInfoPanel : MonoBehaviour
    {
        [Tooltip("The text element to display the player name")]
        [SerializeField]
        private Text _playerNameText;
        [Tooltip("The text element to display the player number")]
        [SerializeField]
        private Text _connIdText;
        [Tooltip("The image that will show if we are the master client or not")]
        [SerializeField]
        private Image _masterClientStatusImage;


        public Text PlayerNameText
        {
            get
            {
                return _playerNameText;
            }

        }

        public Text ConnIdText
        {
            get
            {
                return _connIdText;
            }
        }

        public Image MasterClientStatusImage
        {
            get
            {
                return _masterClientStatusImage;
            }
        }
    }
}