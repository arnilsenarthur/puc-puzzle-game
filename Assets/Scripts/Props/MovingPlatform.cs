using UnityEngine;

namespace Props
{
    public class MovingPlatform : MonoBehaviour
    {
        #region Settings 
        public Vector3 offset;
        #endregion

        #region State
        public Vector3 currentMove => _rigidbody.velocity;
        #endregion

        #region Private Variables
        private Vector3 _startPos;
        private Rigidbody _rigidbody;
        #endregion

        private void Start()
        {
            _startPos = transform.position;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 target = _startPos + Mathf.Sin(Time.time * Mathf.Deg2Rad * 60f) * offset;
            //currentMove = transform.position - target;
            _rigidbody.MovePosition(target);
            _rigidbody.MoveRotation(transform.rotation * Quaternion.Euler(0f, Time.fixedDeltaTime * 15f, 0f));
        }
    }
}