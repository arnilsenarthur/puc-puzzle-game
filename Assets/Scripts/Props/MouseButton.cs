using UnityEngine;

namespace Props
{
    public class MouseButton : MonoBehaviour
    {
        #region Settings
        public float clickForce = 0.5f;
        #endregion

        #region Private Fields
        private Button _button;
        private bool _pressed = false;
        #endregion

        private void OnEnable()
        {
            this._button = transform.parent.GetComponent<Button>();
        }

        public void OnClick()
        {
            _button.Press();  
        }
    }
}