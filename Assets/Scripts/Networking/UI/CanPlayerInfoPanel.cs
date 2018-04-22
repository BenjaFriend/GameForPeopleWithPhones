using UnityEngine;
using UnityEngine.UI;

namespace Com.PodSquad.GDPPNF
{
    public class CanPlayerInfoPanel : PlayerInfoPanel
    {
        [Tooltip("")]
        [SerializeField]
        private Sprite _poppedSprite;

        [Tooltip("The can image that we can set to popped if we want to")]
        [SerializeField]
        private Image _canImage;

        public Image CanImage
        {
            get
            {
                return _canImage;
            }

            set
            {
                _canImage = value;
            }
        }

        /// <summary>
        /// Set this players can image to popped
        /// </summary>
        public void PopCan()
        {
            if (_canImage == null || _poppedSprite == null) return;
            _canImage.sprite = _poppedSprite;
        }
    }
}