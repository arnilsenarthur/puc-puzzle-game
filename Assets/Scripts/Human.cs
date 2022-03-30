using UnityEngine;

public class Human : MonoBehaviour
{
    #region State
    [Header("State")]
    public Vector3 velocity = Vector3.zero;
    public bool isGrounded => _isGrounded;
    #endregion

    #region Settings
    [Header("Settings")]
    [Range(-0.01f, -1)]
    public float groundSnap = -0.025f;
    [Range(0f, 30f)]
    public float gravityForce = 9.81f;
    #endregion

    #region Private Fields
    private CharacterController _controller;
    private float _pushPower = 2.0f;
    private bool _isRightSlope = false;
    private Vector3 _groundNormal = Vector3.zero;
    private bool _isGrounded = false;
    private ObjectPickup _objectPickup;
    #endregion

    #region Unity Methods
    void Start()
    {
        _objectPickup = GetComponent<ObjectPickup>();
        _controller = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        _controller.Move(new Vector3(0, groundSnap, 0f));

        if (_isGrounded = _controller.isGrounded)
        {
            if (!_isRightSlope)
            {
                velocity.x += (1f - _groundNormal.y) * _groundNormal.x * (1f - 0.3f);
                velocity.z += (1f - _groundNormal.y) * _groundNormal.z * (1f - 0.3f);
            }

            if (velocity.y < 0.1f)
                velocity.y = 0;
        }
        else
        {
            velocity.y -= gravityForce * Time.fixedDeltaTime;
        }

        
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, _controller.radius / 1.5f, Vector3.down, out hit, 1.1f))
        {
            Rigidbody rb;
            if ((rb = hit.collider.gameObject.GetComponent<Rigidbody>()) != null && !rb.gameObject.CompareTag("Movable"))
            {
                if (_objectPickup == null || _objectPickup.grabbed != rb)
                {
                    Vector3 tg = transform.position + rb.velocity * Time.deltaTime;
                    tg = RotatePointAroundPivot(tg,rb.position,rb.angularVelocity);
                    _controller.Move(tg - transform.position);
                    //_controller.Move(rb.velocity * Time.fixedDeltaTime);
                }
            }
        }
    }

    void Update()
    {
        _controller.Move(velocity * Time.deltaTime);
    }

    #endregion

    #region Private Methods
    private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        Vector3 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        velocity -= hit.normal * Vector3.Dot(velocity, hit.normal);

        _isRightSlope = (Vector3.Angle(Vector3.up, hit.normal) <= _controller.slopeLimit);
        _groundNormal = hit.normal;

        Rigidbody body = hit.collider.attachedRigidbody;

        if(hit.collider.gameObject.tag == "Grabbable")
            return;

        if (hit.normal.y < -0.6f)
        {
            velocity.y = 0f;
        }

        if (body == null || body.isKinematic) return;

        if (hit.moveDirection.y < -0.3f)
        {
            if (body.GetComponentInParent<Props.Button>() != null)
            {
                body.velocity = Vector3.down * _pushPower;
            }

            return;
        }

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        if (body.GetComponentInParent<Props.Button>() != null)
            body.velocity = pushDir * 0.5f;
        else
            body.velocity = pushDir * _pushPower;
    }
    #endregion
}
