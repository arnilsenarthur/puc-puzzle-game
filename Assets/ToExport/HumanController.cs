using UnityEngine;

public class HumanController : MonoBehaviour
{
    #region References
    public new Camera camera;
    #endregion

    #region Settings
    [Header("Settings")]
    [Range(0f, 10f)]
    public float walkSpeed = 6f;
    [Range(0f, 10f)]
    public float lookSpeed = 3f;
    #endregion

    #region Private Fields
    private Human _human;
    private float _rotY = 0;
    private Vector3 _input = Vector3.zero;
    #endregion

    #region Unity Methods
    private void Start()
    {
        _human = GetComponent<Human>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Update()
    {
        Vector3 newInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        _input = Vector3.Lerp(_input, newInput, Time.deltaTime * 8f);

        if (_human.isGrounded)
        {
            Vector3 vel = (transform.forward * _input.z + transform.right * _input.x) * walkSpeed;
            vel.y = _human.velocity.y;
            _human.velocity = Vector3.Lerp(_human.velocity, vel, Time.deltaTime * 16f);

            if (Input.GetKey(KeyCode.Space))
            {
                _human.velocity = new Vector3(_human.velocity.x, 8f, _human.velocity.z);
            }
        }
        else
        {
            Vector3 vel = (transform.forward * _input.z + transform.right * _input.x) * walkSpeed;
            vel.y = _human.velocity.y;

            _human.velocity = Vector3.Lerp(_human.velocity, vel, Time.deltaTime * 4f);
        }

        transform.localEulerAngles += new Vector3(0, 1, 0) * Input.GetAxis("Mouse X") * lookSpeed;

        _rotY += Input.GetAxis("Mouse Y") * lookSpeed;
        _rotY = Mathf.Clamp(_rotY, -90, 90);
        camera.transform.localEulerAngles = new Vector3(-_rotY, 0, 0);
    }
    #endregion
}
