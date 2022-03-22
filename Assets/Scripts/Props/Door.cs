using UnityEngine;
using Game.Data;

namespace Props
{
    public class Door : MonoBehaviour
    {
        #region Settings
        public Vector3 openOffset = new Vector3(0, 0, 0);
        public float speed = 2f;
        public LayerMask layer;
        public float delayToCloseAfterObstacle = 0.5f;
        public Door parentDoor;
        #endregion

        #region State
        public BoolValue open = new BoolValue { value = false };
        public bool IsReallyClosed => _isReallyClosed;
        #endregion

        #region Private Fields
        private Vector3 _startLocalPosition;
        private BoxCollider _collider;
        private bool _isReallyClosed = false;
        private bool _dodgingObstacle
        {
            get => parentDoor ? parentDoor.__dodgingObstacle : __dodgingObstacle;
            set
            {
                if (parentDoor)
                    parentDoor.__dodgingObstacle = value;
                else
                    __dodgingObstacle = value;
            }
        }
        private bool __dodgingObstacle = false;

        private float _waitingToCloseAgain
        {
            get => parentDoor ? parentDoor.__waitingToCloseAgain : __waitingToCloseAgain;
            set
            {
                if (parentDoor)
                    parentDoor.__waitingToCloseAgain = value;
                else
                    __waitingToCloseAgain = value;
            }
        }

        private float __waitingToCloseAgain = 0;
        #endregion

        void Start()
        {
            _startLocalPosition = transform.localPosition;
            _collider = GetComponent<BoxCollider>();

            if (open.value)
                transform.localPosition = _startLocalPosition + openOffset;
        }

        void FixedUpdate()
        {
            Vector3 targetPosition = _startLocalPosition;

            if (open.value || _dodgingObstacle)
                targetPosition += openOffset;

            targetPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, Time.fixedDeltaTime * speed);

            bool equals = transform.localPosition == targetPosition;
            if (equals)
            {
                if (_waitingToCloseAgain <= 0)
                    _dodgingObstacle = false;
                else
                    _waitingToCloseAgain -= Time.fixedDeltaTime;
            }
            else
            {
                Vector3 futurePosition = targetPosition + (transform.parent != null ? transform.parent.position : Vector3.zero);

                if (!Physics.CheckBox(futurePosition, transform.lossyScale / 2f, Quaternion.identity, layer))
                {
                    transform.localPosition = targetPosition;
                }
                else
                {
                    _waitingToCloseAgain = delayToCloseAfterObstacle;
                    _dodgingObstacle = true;
                }
            }

            _isReallyClosed = equals && !open.value && !_dodgingObstacle;
        }

        private void OnDrawGizmos()
        {
            //draw wire cube in target position
            Gizmos.color = Color.red;
            Vector3 st = Application.isPlaying ? _startLocalPosition : transform.localPosition;
            Vector3 endPosition = openOffset + st + (transform.parent != null ? transform.parent.position : Vector3.zero);
            Gizmos.DrawWireCube(endPosition, transform.lossyScale);
        }
    }
}