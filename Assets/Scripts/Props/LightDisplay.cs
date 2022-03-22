using UnityEngine;
using Game.Data;
using TMPro;

namespace Props
{
    public class LightDisplay : MonoBehaviour
    {
        #region References
        public BoolValue isOn;
        public Renderer lightPart;
        public TMP_Text textLabel;
        #endregion

        #region Settings
        public Color colorOn = Color.red;
        public Color colorOff = Color.black;
        #endregion

        private void OnEnable() 
        {
            if(lightPart == null)
                lightPart = GetComponentInChildren<Renderer>();

            if(isOn.variableValue != null)
                isOn.variableValue.OnValueChanged.AddListener(OnValueChanged);

            OnValueChanged();
        }

        private void OnDisable() 
        {
            if(isOn.variableValue != null)
                isOn.variableValue.OnValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged() 
        {
            lightPart.material.color = isOn.value ? colorOn : colorOff;

            if(textLabel != null)
                textLabel.text = isOn.value ? "ON" : "OFF";
        }
    }
}