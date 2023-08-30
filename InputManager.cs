using UnityEngine;

[AddComponentMenu("Input/Manager")]
public class InputManager : MonoBehaviour
{
    [Header("Movement")]
    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode right = KeyCode.D;
    public KeyCode left = KeyCode.A;
    [Header("Mouse")] public bool invertX = false;
    public bool invertY = true;
    public Vector2 sensitivity = Vector2.one;
    private static InputManager _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static Vector2 MovementDirection()
    {
        float x = 0;
        float y = 0;
        if (Input.GetKey(_instance.forward)) y += 1;
        if (Input.GetKey(_instance.back)) y -= 1;

        if (Input.GetKey(_instance.right)) x += 1;
        if (Input.GetKey(_instance.left)) x -= 1;

        return new Vector2(x, y);
    }

    public static Vector2 Mouse()
    {
        float x = Input.GetAxisRaw("Mouse X") * _instance.sensitivity.x * (_instance.invertX ? -1 : 1);
        float y = Input.GetAxisRaw("Mouse Y") * _instance.sensitivity.y * (_instance.invertY ? -1 : 1);
        return new Vector2(x,y);
    }

    //More can be added this is just a template :)
}
