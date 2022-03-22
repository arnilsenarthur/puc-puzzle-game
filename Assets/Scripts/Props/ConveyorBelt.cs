using UnityEngine;
using Game.Data;

namespace Props
{
    public class ConveyorBelt : MonoBehaviour
    {
        #region State
        public BoolValue on = new BoolValue{value = true};
        #endregion

        #region Settings
        public float speed;
        public float visualSpeedScalar;
        #endregion

        #region Private Fields
        private float _speedMultipler = 1f;
        private Renderer _renderer;
        private float _currentScroll;
        private Rigidbody _rigidbody;
        #endregion

        private void OnEnable() 
        {
            _renderer = GetComponent<Renderer>();
            _renderer.material.mainTextureScale = new Vector2 (1, transform.lossyScale.z * 10f * visualSpeedScalar);
        }

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 pos = transform.position;
            
            _speedMultipler = Mathf.Lerp(_speedMultipler, on.value ? 1f : 0f, Time.deltaTime * 5f);

            _rigidbody.position -= transform.forward * speed * Time.fixedDeltaTime * _speedMultipler;
            _rigidbody.MovePosition(pos);
        }

        private void Update()
        {
            _currentScroll = _currentScroll + Time.deltaTime * speed * visualSpeedScalar * _speedMultipler;
            _renderer.material.mainTextureOffset = new Vector2(0, _currentScroll);
        }
    }
}