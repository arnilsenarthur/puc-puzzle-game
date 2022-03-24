using UnityEngine;
using Game.Data;

namespace Props
{
    public class WeightScale : MonoBehaviour
    {
        #region References
        public new Rigidbody rigidbody;
        #endregion

        #region Settings
        public float calibratedMax = 0.85f;
        public float weightCalibration = 20f;
        public AnimationCurve valueCalibration;
        public float unit = 0.25f;
        #endregion

        #region State
        public FloatValue value;
        public float push;
        public float targetWeight;
        public BoolValue targetWeightResponse;
        #endregion

        #region Private Variables
        private Vector3 _initialPosition;
        private float _defaultPosPush = 3f;
        #endregion

        private void Start()
        {
            _initialPosition = rigidbody.transform.position;
            _defaultPosPush = Vector3.Distance(_initialPosition, transform.position);
        }

        private void FixedUpdate()
        {
            Vector3 localVelocity = transform.InverseTransformVector(rigidbody.velocity);

            float offsetY = Vector3.Distance(rigidbody.transform.position, transform.position);
            push = (offsetY / _defaultPosPush);

            float notCalibratedValue = (1f - Mathf.Clamp01(push / calibratedMax)) * weightCalibration;
            float v = notCalibratedValue * valueCalibration.Evaluate(notCalibratedValue);
            v = Mathf.Round(v / unit) * unit;

            value.value = Mathf.Lerp(value.value, v, Time.fixedDeltaTime * 50f);

            targetWeightResponse.value = value.value == targetWeight;

            rigidbody.angularVelocity = Vector3.zero;
        }
    }
}