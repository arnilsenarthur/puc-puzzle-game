using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Data
{
    /// <summary>
    /// Main variable base class
    /// </summary>
    public class Variable : ScriptableObject
    {
        #region Public Fields
        public bool named = false;
        public string variableName;

        public UnityEvent OnValueChanged;
        #endregion

        #region Public Properties
        public virtual object AsObject => null;
        public virtual Type ValueType => null;
        #endregion

        #region Public Methods
        public override string ToString() => "?";
        public virtual bool TryToSetValue(object value) { return false; }
        #endregion
    }

    public class Variable<T> : Variable
    {
        #region Private Fields
        [SerializeField]
        private T value;
        #endregion

        #region Public Properties
        public T Value
        {
            get => value;
            set
            {
                this.value = (T)value;
                OnValueChanged.Invoke();
            }
        }

        public override Type ValueType => typeof(T);

        public override object AsObject => value;
        #endregion

        #region Public Methods
        public override bool TryToSetValue(object value)
        {
            if (value.GetType() != typeof(T))
                return false;

            Value = (T)value;
            return true;
        }

        public override string ToString() => value.ToString();
        #endregion
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(Variable<>), true)]
    public class VariableEditor : Editor
    {
        SerializedProperty value;
        SerializedProperty named;
        SerializedProperty variableName;

        void OnEnable()
        {
            value = serializedObject.FindProperty("value");
            named = serializedObject.FindProperty("named");
            variableName = serializedObject.FindProperty("variableName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            object before = (target as Variable).AsObject;
            EditorGUILayout.PropertyField(value);
            object after = (target as Variable).AsObject;

            if (before != after)
                (target as Variable).OnValueChanged.Invoke();

            EditorGUILayout.PropertyField(named);

            if (named.boolValue)
                EditorGUILayout.PropertyField(variableName);

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}
