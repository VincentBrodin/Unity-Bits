using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Shake Settings")] 
    public float magnitude;
    public float maxDistance;
    public float speed;
    public float baseShakeDuration;
    
    [Header("References")]
    public Transform shakeObject;
    
    private float _shakeDuration;
    private float _currentMagnitude;
    
    private Vector3 _shakePosition;
    
    private static CameraController _instance;
    
    private void Awake()
    {
        _instance = this;
    }
    
    private void Update()
    {
        if (_shakeDuration > 0)
        {
            _shakePosition = Vector3.Lerp(_shakePosition, Random.insideUnitSphere * _currentMagnitude,
                speed * Time.deltaTime);
            shakeObject.localPosition = _shakePosition;
			
            _shakeDuration -= Time.deltaTime;
        }
        else
        {
            _shakeDuration = 0f;
            shakeObject.localPosition = Vector3.zero;
        }
    }

    public static void Shake()
    {
        _instance._currentMagnitude = _instance.magnitude;
        _instance._shakeDuration = _instance.baseShakeDuration;
    }
    
    public static void ShakeAtPosition(Vector3 from, Vector3 to, float size)
    {
        float distance = Vector3.Distance(from, to);
      
        if(distance > _instance.maxDistance) return;

        float distancePercent = 1-(distance / _instance.maxDistance);

        _instance._currentMagnitude = distancePercent * size * _instance.magnitude;
        _instance._shakeDuration = _instance.baseShakeDuration;
    }
}
