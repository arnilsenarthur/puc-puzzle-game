using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using TMPro;
using Game.Data;

namespace Game.UI
{
    public class Label : VariableDisplay
    {
        static readonly Regex translationPattern = new Regex(@"\[([^\]]+)\]", RegexOptions.Compiled);

        [Multiline]
        public string format = "%s";

        private TMP_Text _text;

        public string Text
        {
            get
            {             
                return _text.text;
            }

            set
            {

                _text.text = value;
            }
        }

        public override void OnEnable()
        {
            _text = GetComponent<TMP_Text>();
            //Language.OnChange.AddListener(OnValueChanged);
            base.OnEnable();
        }

        public override void OnDisable()
        {
            //Language.OnChange.RemoveListener(OnValueChanged);
            base.OnDisable();
        }

        public override void OnValueChanged()
        {
            try
            {   
                string text = format;

                //Replace values
                object[] values = variables.Select(v => v.AsObject).ToArray();
                text = string.Format(text, values);

                Text = text;
            }
            catch
            {
                Text = "-/-";
            }
        }
    }

    public abstract class VariableDisplay : MonoBehaviour
    {
        public Variable[] variables;

        public virtual void OnEnable()
        {
            foreach(Variable variable in variables)
            {
                if(variable != null)
                    variable.OnValueChanged.AddListener(OnValueChanged);
            }

            OnValueChanged();
        }

        public virtual void OnDisable()
        {
            foreach(Variable variable in variables)
            {
                if(variable != null)
                    variable.OnValueChanged.RemoveListener(OnValueChanged);
            }
        }

        public virtual void OnValueChanged() { }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(Label), true)]
    [CanEditMultipleObjects]
    public class LabelEditor : Editor
    {
        SerializedProperty format;
        SerializedProperty variables;

        void OnEnable()
        {
            format = serializedObject.FindProperty("format");
            variables = serializedObject.FindProperty("variables");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(format);
            EditorGUILayout.PropertyField(variables);

            if (serializedObject.ApplyModifiedProperties())
            {
                Label t = target as Label;

                if (Application.isPlaying)
                    t.OnValueChanged();
                else
                    t.Text = t.format;
            }
        }
    }
    #endif
}