using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField]private bool grounded = false;
    [Space]
    [SerializeField]private float maxSlopeAngle = 35f;
    [SerializeField]private LayerMask groundLayer = -1;
    
    private bool _cancellingGrounded;
    private const float Delay = 3f;

    private Vector3 _normalVector;

    public Vector3 GroundNormal()
    {
        return _normalVector;
    }
    
    public bool Grounded()
    {
        return grounded;
    }
    
    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }
    
    private void OnCollisionStay(Collision other)
    {
        int layer = other.gameObject.layer;
        if (groundLayer != (groundLayer | (1 << layer))) return;

        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            if (IsFloor(normal)) {
                grounded = true;
                _cancellingGrounded = false;
                _normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        if (!_cancellingGrounded) {
            _cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * Delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

}
