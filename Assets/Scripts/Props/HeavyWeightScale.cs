using UnityEngine;
using Game.Data;

public class HeavyWeightScale : MonoBehaviour
{
    #region References
    public Transform tray;
    #endregion

    #region State
    public FloatValue value;
    #endregion

    #region Settings
    public FloatValue unit = new FloatValue{value = 1f};
    public Collider[] collidersToIgnore;
    public float roundUnit = 0.1f;
    public float targetWeight;
    public BoolValue targetWeightResponse;
    #endregion

    #region Private Fields
    private Rigidbody _rigidbody;
    private Vector3 _startPos;
    private Vector3 _trayPos;
    private int _fm = 0;
    private float _wg = 0;
    private float _lastWeightSum = 0;
    private float _weightSum = 0;
    #endregion

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _startPos = transform.position;

        if(tray != null)
            _trayPos = tray.position;

        value.value = 0;
	
	Collider collider = GetComponent<Collider>();

	foreach(Collider col in collidersToIgnore)
	{
		Physics.IgnoreCollision(collider,col);
	}
    }

    private void OnCollisionStay(Collision other)
    {
        float f = other.impulse.magnitude;
        _weightSum += other.impulse.magnitude * 5.095f;
    }

    private void FixedUpdate()
    {
        if (!_rigidbody.isKinematic)
        {
            _lastWeightSum = _weightSum;
            _weightSum = 0;
        }

         _wg = Mathf.Lerp(_wg,Mathf.Round(_lastWeightSum / 2f), Time.fixedDeltaTime * 10f);    
        value.value = (Mathf.Round(_wg/roundUnit) * roundUnit) * unit.value;

        targetWeightResponse.value = value.value == targetWeight;

        _rigidbody.isKinematic = _fm % 5 != 0;
        _fm++;

        _rigidbody.MovePosition(_startPos);
        _rigidbody.velocity = Vector3.zero;
    }

    private void Update() 
    {
        if(tray != null)
            tray.position = _trayPos;
    }
}
