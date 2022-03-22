using UnityEngine;
using Game.Data;

namespace Props
{
    public class Gate : MonoBehaviour
    {
        #region Settings
        public Vector3 openOffset = new Vector3(0, 0, 0);
        public float speed = 3f;
        public float obstacleOpenSpeed = 0.75f;
        public float waitAfterObstacle = 1f;
        public bool ignoreObstacles = false;
        #endregion

        #region State
        public BoolValue open = new BoolValue { value = false };
        public float length => openOffset.magnitude;
        public bool IsRealClosed => _progress == 0f && !open.value;
        #endregion

        #region Private Fields
        private Vector3 _startPosition;
        private float _progress = 0f;
        private BoxCollider _collider;
        private bool _openingDodgingObstacle = false;
        private float _waitToCloseAgain = 0f;
        private Vector3 _lastPosition;
        #endregion

        private void Start()
        {
            _startPosition = transform.position;

            _collider = GetComponent<BoxCollider>();

            if (open.value)
            {
                transform.position = _startPosition + openOffset;
            }

            _lastPosition = transform.position;
        }

        private void Update()
        {
            _startPosition += (transform.position - _lastPosition);

            if (open.value)
            {
                _openingDodgingObstacle = false;
                _progress += Time.deltaTime * speed;
            }
            else
            {
                if (_openingDodgingObstacle)
                {
                    _progress += Time.deltaTime * obstacleOpenSpeed;

                    if (_progress >= 1f)
                    {
                        _waitToCloseAgain -= Time.deltaTime;

                        if (_waitToCloseAgain <= 0f)
                            _openingDodgingObstacle = false;
                    }
                }
                else
                {
                    Vector3 size = new Vector3(
                        _collider.size.x * transform.lossyScale.x,
                        _collider.size.y * transform.lossyScale.y,
                        _collider.size.z * transform.lossyScale.z
                    );

                    float p = _progress - Time.deltaTime * speed;
                    bool blocked = false;

                    if (!ignoreObstacles)
                    {
                        Vector3 futurePos = Vector3.Lerp(_startPosition, _startPosition + openOffset, p);
                        Collider[] col = Physics.OverlapBox(futurePos, size / 2f, transform.rotation);

                        foreach (Collider c in col)
                        {
                            if (c.GetComponent<Rigidbody>() != null || c.GetComponent<CharacterController>() != null)
                            {
                                blocked = true;
                                break;
                            }
                        }
                    }

                    if (blocked)
                    {
                        _waitToCloseAgain = waitAfterObstacle;
                        _openingDodgingObstacle = true;
                    }
                    else _progress = p;
                }
            }

            _progress = Mathf.Clamp01(_progress);

            transform.position = Vector3.Lerp(_startPosition, _startPosition + openOffset, _progress);
            _lastPosition = transform.position;
        }

    }
}