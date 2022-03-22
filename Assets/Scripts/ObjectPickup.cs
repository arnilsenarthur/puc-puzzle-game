using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    #region References
    public new Camera camera;
    public Rigidbody grabbed;
    public new LineRenderer renderer;
    public LineRenderer dropRenderer;
    #endregion

    #region Settings
    public float grabForce = 500f;
    public float maxGrabDistance = 15f;
    public float minGrabDistance = 2f;
    public float scrolSpeed = 3f;
    #endregion

    #region Internal Variables 
    private Vector3 _originalScreenTargetPosition;
    private Vector3 _originalRigidbodyPos;
    private float _selectionDistance;
    #endregion

    void Update()
    {
        if (!camera)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            grabbed = GetRigidbodyFromMouseClick();
        }
        if (Input.GetMouseButtonUp(0) && grabbed)
        {
            grabbed.velocity +=  camera.transform.forward * grabbed.velocity.sqrMagnitude/15f;
            
            //Limit velocity
            Vector3 vel = grabbed.velocity;
            float mag = Mathf.Clamp(vel.magnitude,0,50);
            grabbed.velocity = vel.normalized * mag;

            grabbed = null;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        scroll = Mathf.Clamp(scroll,-1,1);

        if (scroll != 0)
        {
            _selectionDistance += scroll * scrolSpeed;
        }

        _selectionDistance = Mathf.Clamp(_selectionDistance, minGrabDistance, maxGrabDistance);

        renderer.enabled = grabbed;
        dropRenderer.enabled = false;

        if (grabbed)
        {
            Vector3 from = camera.transform.position - transform.up * 0.21f + camera.transform.forward * 0.5f;

            float xOf = -Mathf.Clamp((camera.WorldToScreenPoint(grabbed.transform.position).x - 50 - Screen.width / 2f) / 100f, -1f, 1f);

            Vector3 control = camera.transform.position - transform.up * 0.05f + camera.transform.right * xOf + camera.transform.forward * 1f;
            Vector3 to = grabbed.transform.position;

            dropRenderer.positionCount = 2;
            dropRenderer.SetPosition(0, grabbed.transform.position);
            dropRenderer.SetPosition(1, grabbed.transform.position + Vector3.down * 5f);

            DrawQuadraticBezierCurve(from, control, to);
        }
    }

    void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        renderer.positionCount = 10;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < renderer.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            renderer.SetPosition(i, B);
            t += (1 / (float)renderer.positionCount);
        }
    }

    void FixedUpdate()
    {
        if (grabbed)
        {
            Vector3 mousePositionOffset = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _selectionDistance)) - _originalScreenTargetPosition;
            grabbed.velocity = (_originalRigidbodyPos + mousePositionOffset - grabbed.transform.position) * grabForce * Time.deltaTime;
            grabbed.angularVelocity = Vector3.Lerp(grabbed.angularVelocity, Vector3.zero, Time.deltaTime * 10f);
        }
    }

    Rigidbody GetRigidbodyFromMouseClick()
    {
        RaycastHit hitInfo = new RaycastHit();
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        bool hit = Physics.Raycast(ray, out hitInfo);
        if (hit)
        {
            if (hitInfo.collider.gameObject.GetComponent<Rigidbody>())
            {
                if (hitInfo.collider.CompareTag("Grabbable") && hitInfo.distance <= maxGrabDistance)
                {
                    _selectionDistance = Vector3.Distance(ray.origin, hitInfo.point);
                    _originalScreenTargetPosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _selectionDistance));
                    _originalRigidbodyPos = hitInfo.collider.transform.position;
                    return hitInfo.collider.gameObject.GetComponent<Rigidbody>();
                }
            }
        }

        return null;
    }
}