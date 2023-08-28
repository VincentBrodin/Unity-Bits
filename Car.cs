using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Car Settings")] 
    public float motorSpeed = 8000;

    [Header("Steering Settings")] 
    public float steeringAngle = 45;
    [Header("Wheel Settings")]
    public float lateralGrip = .75f;
    public float lateralDriftGrip = .25f;
    public float longitudeGrip = .4f;
    public float slipCoefficient = 4250f;
    [Space]
    public Wheel[] wheels;
    public float wheelHeight = .25f;
    public float maxWheelDrop = .25f;
    public float wheelRadius = .35f;
    public float rpmBoost = 120f;
    
    [Header("Suspension Settings")]
    public float springConstant = 1500f;
    public float massScale = 20f;
    public float dampingCoefficient = 100f;
    public LayerMask groundLayers;
    [Header("Audio")] 
    public AudioSource engineSound;
 
    [Space]
    private Rigidbody _rigidbody;

    private float _throttle;
    private float _steering;
    private float _currentSteering;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.grounded && wheel.raycastHit)
            {
                #region Suspension
                float displacement = Vector3.Distance(wheel.desiredPosition, wheel.position);
                if ((wheel.desiredPosition.y - wheel.position.y) < 0)
                {
                    displacement *= -1;
                }                

                float velocity = _rigidbody.GetPointVelocity(wheel.hitPosition).y;

                float suspensionForce = -(springConstant * displacement * massScale + dampingCoefficient * velocity * massScale);
               
                _rigidbody.AddForceAtPosition(suspensionForce * wheel.upDirection, wheel.anchor.position);
                #endregion
                
                #region Motor

                _rigidbody.AddForceAtPosition(wheel.direction.forward * motorSpeed * _throttle, wheel.hitPosition);
                
                #endregion
                
                #region Friction
                Vector3 pointVelocity = _rigidbody.GetPointVelocity(wheel.hitPosition);
                Vector3 lateralVelocity = Vector3.Project(pointVelocity, wheel.direction.right);
                Vector3 longitudeVelocity = Vector3.Project(pointVelocity, wheel.direction.forward);

                float mass = _rigidbody.mass;
                
                Vector3 lateralFrictionForce = mass * lateralGrip * -lateralVelocity;
                Vector3 longitudeFrictionForce = mass * longitudeGrip * -longitudeVelocity;
                
                bool traction = lateralFrictionForce.magnitude < slipCoefficient;
                if (wheel.steering || traction)
                {
                    _rigidbody.AddForceAtPosition(lateralFrictionForce, wheel.hitPosition);
                    wheel.drifting = false;
                }
                else
                {
                    Vector3 lateralDriftFrictionForce = mass * lateralDriftGrip * -lateralVelocity;
                    _rigidbody.AddForceAtPosition(lateralDriftFrictionForce, wheel.hitPosition);
                    wheel.drifting = true;
                }
                
                _rigidbody.AddForceAtPosition(longitudeFrictionForce, wheel.hitPosition);
                #endregion
            }
        }
        
        foreach (var wheel in wheels)
        {
            wheel.model.localScale = new Vector3(wheelRadius * 2, wheelRadius * 2, wheelRadius * 2);
        }
    }

    private void Update()
    {
        _currentSteering = _steering * steeringAngle;

        foreach (var wheel in wheels)
        {
            if(!wheel.steering) continue;
            Vector3 modelRotation = wheel.model.localRotation.eulerAngles;
            wheel.model.localRotation = Quaternion.Euler(modelRotation.x, _currentSteering, modelRotation.z);
            
            Vector3 directionRotation = wheel.direction.localRotation.eulerAngles;
            wheel.direction.localRotation = Quaternion.Euler(directionRotation.x, _currentSteering, directionRotation.z);
        }
    }

    private void LateUpdate()
    {
        float averageRpm = 0;
        foreach (var wheel in wheels)
        {
            //Raycast
            Vector3 anchorPosition = wheel.anchor.position;
            Vector3 anchorUpDirection = wheel.anchor.up;
            wheel.downDirection = -anchorUpDirection;
            wheel.upDirection = anchorUpDirection;
            
            wheel.raycastHit = Physics.Raycast(anchorPosition, wheel.downDirection, out wheel.hit, Mathf.Infinity, groundLayers);

            //Get Wheel Positions
            Vector3 wheelHitPoint = wheel.hit.point;
            Vector3 wheelPosition = wheelHitPoint;
            wheelPosition += wheelRadius * wheel.upDirection;
            
            //Constrain Wheel Positions
            Vector3 desiredWheelPosition = anchorPosition;
            desiredWheelPosition += wheelHeight * wheel.downDirection;
            wheel.desiredPosition = desiredWheelPosition;
            if (wheel.desiredPosition.y - wheelPosition.y > maxWheelDrop)
            {
                wheelPosition = wheel.desiredPosition;
                wheelPosition += maxWheelDrop * wheel.downDirection;
                wheel.grounded = false;
            }
            else
                wheel.grounded = true;

            Vector3 wheelHitPosition = wheelPosition;
            wheelHitPosition += wheelRadius * wheel.downDirection;
            wheel.hitPosition = wheelHitPosition;
            
            wheel.position = wheelPosition;
            wheel.wheel.position = wheel.position;
            
            //Effects
            bool isDrifting = (wheel.grounded && wheel.drifting);
            wheel.skidMarks.transform.position = wheel.hitPosition;
            wheel.tireSmoke.transform.position = wheel.hitPosition;
            ParticleSystem.EmissionModule tireSmokeEmissionModule =  wheel.tireSmoke.emission;
            wheel.skidMarks.emitting = isDrifting;
            tireSmokeEmissionModule.enabled = isDrifting;

            float distance = Vector3.Distance(wheel.hitPosition, wheel.lastHitPosition);
            float circumference = wheelRadius * 3.14f * 2f;
            float rpm = (distance / circumference) * rpmBoost;
            wheel.rpm += -rpm;
            averageRpm += rpm;
            wheel.rpmModel.localRotation = Quaternion.Euler(0, wheel.rpm, 0);
            wheel.lastHitPosition = wheel.hitPosition;
        }

        averageRpm /= wheels.Length;
        averageRpm /= Time.deltaTime;
        float rpmPitch = (averageRpm / 1000) + 1;
        engineSound.pitch = rpmPitch;
    }

    public void SetThrottle(float throttle)
    {
        _throttle = throttle;
    }

    public void SetSteering(float steering)
    {
        _steering = steering;
    }

    [Serializable]
    public class Wheel
    {
        public string wheelName;
        [Space] 
        public bool steering;
        [Space]
        public Transform anchor;
        public Transform wheel;
        public Transform direction;
        public Transform model;
        public Transform rpmModel;
        [Header("Effects")] 
        public TrailRenderer skidMarks;
        public ParticleSystem tireSmoke;
        [Header("Checks")] 
        public bool grounded;
        public bool raycastHit;
        public bool drifting;
        [HideInInspector]public Vector3 position;
        [HideInInspector]public Vector3 hitPosition;
        [HideInInspector]public Vector3 desiredPosition;
        [HideInInspector]public Vector3 downDirection;
        [HideInInspector]public Vector3 upDirection;
        [HideInInspector]public Vector3 lastHitPosition;
        [HideInInspector]public float rpm;
        public RaycastHit hit;
        
    }

    private void OnGUI()
    {
        int precision = 10;
        
        float width = 1500;
        float height = 50;
        int fontSize = 25;
        float x = 50;
        float y = 50f;
        float spacing = 50f;
        GUI.color = Color.red;
        GUI.skin.label.fontSize = fontSize;
        Vector3 averageVelocity = Vector3.zero;
        foreach (var wheel in wheels)
        {
            Rect rect = new Rect
            {
                width = width,
                height = height,
                x = x,
                y = y
            };
            y += spacing;
            
            //VALUES
            Vector3 pointVelocity = _rigidbody.GetPointVelocity(wheel.hitPosition);
            averageVelocity += pointVelocity;
            Vector3 lateralVelocity = Vector3.Project(pointVelocity, wheel.direction.right);
            Vector3 longitudeVelocity = Vector3.Project(pointVelocity, wheel.direction.forward);
            float mass = _rigidbody.mass;
            Vector3 lateralFrictionForce = mass * lateralGrip * -lateralVelocity;
            Vector3 longitudeFrictionForce = mass * longitudeGrip * -longitudeVelocity;
            bool traction = lateralFrictionForce.magnitude < slipCoefficient;
            bool relativeTraction = wheel.steering || traction;
            
            
            string text = $"{wheel.wheelName} | Velocity {Mathf.Round(pointVelocity.magnitude*precision)/precision} | Lateral Friction {Mathf.Round(lateralFrictionForce.magnitude*precision)/precision} | Longitude Friction {Mathf.Round(longitudeFrictionForce.magnitude*precision)/precision} | Traction {relativeTraction} | Drifting {wheel.drifting} | Grounded {wheel.grounded}";
            GUI.Label(rect, text);
        }

        averageVelocity /= wheels.Length;
        Rect velRect = new Rect
        {
            width = width,
            height = height,
            x = x,
            y = y
        };
        y += spacing;
        
        string velText = $"Vel {Mathf.Round(averageVelocity.magnitude*precision)/precision}";
        GUI.Label(velRect, velText);
    }

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;
        foreach (var wheel in wheels)
        {
            if (wheel.anchor)
            {
                Gizmos.color = Color.green;
                Vector3 anchorPosition = wheel.anchor.position;
                Gizmos.DrawSphere(anchorPosition, .1f);
                Gizmos.DrawRay(anchorPosition, -wheel.anchor.up * (maxWheelDrop + wheelRadius));
                Gizmos.DrawSphere(wheel.desiredPosition, .1f);
                
                
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(wheel.hit.point, .1f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(wheel.hitPosition, .1f);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(wheel.position, wheel.direction.forward);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(wheel.position, wheel.direction.right);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(wheel.position, wheel.upDirection);

                
                Vector3 pointVelocity = _rigidbody.GetPointVelocity(wheel.hitPosition);
                Vector3 lateralVelocity = Vector3.Project(pointVelocity, wheel.direction.right);
                Vector3 longitudeVelocity = Vector3.Project(pointVelocity, wheel.direction.forward);
                float mass = _rigidbody.mass;
                Vector3 lateralFrictionForce = mass * lateralGrip * -lateralVelocity;
                Vector3 longitudeFrictionForce = mass * longitudeGrip * -longitudeVelocity;
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(wheel.position, lateralFrictionForce.normalized);
                Gizmos.DrawRay(wheel.position, longitudeFrictionForce.normalized);

            }
        }
    }
}
