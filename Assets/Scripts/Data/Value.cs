using UnityEngine;

namespace Game.Data
{
    [System.Serializable]
    public class FloatValue : Value<float>{}

    [System.Serializable]
    public class IntValue : Value<int> {}

    [System.Serializable]
    public class BoolValue : Value<bool> {}

    [System.Serializable]
    public class StringValue : Value<string> {}

    [System.Serializable]
    public class Vector2Value : Value<Vector2>{}

    [System.Serializable]
    public class Vector3Value : Value<Vector3>{}

    [System.Serializable]
    public class QuaternionValue : Value<Quaternion>{}

    [System.Serializable]
    public class ColorValue : Value<Color>{}


    [System.Serializable]
    public class Value<T>
    {
        public T fixedValue;
        public Variable<T> variableValue;

        public T value {
            get {
                if (variableValue != null)
                {
                    return variableValue.Value;
                }
                else
                {
                    return fixedValue;
                }
            }

            set {
                if (variableValue != null)
                {
                    variableValue.Value = value;
                }
                else
                {
                    fixedValue = value;
                }
            }
        }
    }
}
