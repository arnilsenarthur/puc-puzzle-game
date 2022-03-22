using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Game.Data;

namespace Props
{
    public class Button : MonoBehaviour
    {
        #region References
        public Rigidbody buttonPart;  
        #endregion

        #region State
        public BoolValue pressed = new BoolValue { value = false };
        public bool beingPressed = false;
        #endregion

        #region Events
        public UnityEvent onPress;
        public UnityEvent onRelease;
        #endregion

        #region Settings
        public float springForce = 10;
        public bool stayPressed = false;
        public float relaseAfter = 0f;
        public Collider[] CollidersToIgnore;
        public float maxYOffset = 3f;
        public float unpressedMinOffset = -0.1f;
        #endregion

        #region Private Fields
        private bool _holding = false;
        #endregion

        private void Start()
        {
            Collider localCollider = GetComponent<Collider>();
            if (localCollider != null)
            {
                Physics.IgnoreCollision(localCollider, buttonPart.GetComponentInChildren<Collider>());

                foreach (Collider singleCollider in CollidersToIgnore)
                {
                    Physics.IgnoreCollision(localCollider, singleCollider);
                }
            }
        }

        public IEnumerator ReleaseAfter()
        {
            yield return new WaitForSeconds(relaseAfter);
            _holding = false;
        }

        public void Press()
        {
            buttonPart.transform.localPosition = new Vector3(0, unpressedMinOffset - 0.05f, 0);
            buttonPart.velocity = Vector3.zero;
        }

        private void Update()
        {
            float offsetY = buttonPart.transform.localPosition.y;

            buttonPart.transform.localPosition = new Vector3(0, offsetY, 0);
            buttonPart.transform.localEulerAngles = Vector3.zero;

            if (this.beingPressed)
            {
                if (stayPressed || _holding)
                {
                    if(offsetY > unpressedMinOffset)
                    {
                        buttonPart.transform.localPosition = new Vector3(0, unpressedMinOffset, 0);
                    }
                    return;
                }

                if(!this.pressed.value)
                    this.pressed.value = true;
            }

            bool pressed = offsetY < unpressedMinOffset;

            if (pressed != this.beingPressed)
            {
                this.beingPressed = pressed;
                this.pressed.value = pressed;

                if (pressed)
                {
                    
                    onPress.Invoke();

                    if(relaseAfter > 0 && !stayPressed)
                    {
                        _holding = true;
                        StartCoroutine(ReleaseAfter());
                    } 
                }
                else
                    onRelease.Invoke();
            }

            if (offsetY > maxYOffset)
            {
                buttonPart.transform.localPosition = new Vector3(0, maxYOffset, 0);
            }
            else
            {
                buttonPart.AddForce(buttonPart.transform.up * springForce * Time.deltaTime);
            }
        }

        public void SetStayPressed(bool stayPressed)
        {
            this.stayPressed = stayPressed;
        }
    }
}