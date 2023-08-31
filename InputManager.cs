using UnityEngine;

[AddComponentMenu("Input/Manager")]
public class InputManager : MonoBehaviour
{
    [Header("Movement")]
    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode right = KeyCode.D;
    public KeyCode left = KeyCode.A;
    [Space] 
    public KeyCode jump = KeyCode.Space;
    [Header("Mouse")] public bool invertX = false;
    public bool invertY = true;
    public Vector2 sensitivity = Vector2.one;
    
    private static InputManager _instance;
    
    public enum Setting
    {
        Forward,
        Left,
        Right,
        Back,
        Jump,
        SensX,
        SensY,
    }
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

    public static int Jump()
    {
        int j = 0;
        
        if (Input.GetKey(_instance.jump)) j += 1;

        return j;
    }

    public static Vector2 Mouse()
    {
        float x = Input.GetAxisRaw("Mouse X") * _instance.sensitivity.x * (_instance.invertX ? -1 : 1);
        float y = Input.GetAxisRaw("Mouse Y") * _instance.sensitivity.y * (_instance.invertY ? -1 : 1);
        return new Vector2(x,y);
    }

    //More can be added this is just a template :)

    #region Saving Settings
    public static void SaveSettings()
    {
        #region Movement
        //Forward
        if (PlayerPrefs.HasKey(Setting.Forward.ToString()))
        {
            KeyCode forward = GetKeycodeFromString(PlayerPrefs.GetString(Setting.Forward.ToString()));
            _instance.forward = forward;
        }
        else
        {
            PlayerPrefs.SetString(Setting.Forward.ToString(), _instance.forward.ToString());
        }
        
        //Left
        if (PlayerPrefs.HasKey(Setting.Left.ToString()))
        {
            KeyCode left = GetKeycodeFromString(PlayerPrefs.GetString(Setting.Left.ToString()));
            _instance.left = left;
        }
        else
        {
            PlayerPrefs.SetString(Setting.Left.ToString(), _instance.left.ToString());
        }
        
        //Right
        if (PlayerPrefs.HasKey(Setting.Right.ToString()))
        {
            KeyCode right = GetKeycodeFromString(PlayerPrefs.GetString(Setting.Right.ToString()));
            _instance.right = right;
        }
        else
        {
            PlayerPrefs.SetString(Setting.Right.ToString(), _instance.right.ToString());
        }
        
        //Back
        if (PlayerPrefs.HasKey(Setting.Back.ToString()))
        {
            KeyCode back = GetKeycodeFromString(PlayerPrefs.GetString(Setting.Back.ToString()));
            _instance.back = back;
        }
        else
        {
            PlayerPrefs.SetString(Setting.Back.ToString(), _instance.back.ToString());
        }
        
        //Jump
        if (PlayerPrefs.HasKey(Setting.Jump.ToString()))
        {
            KeyCode jump = GetKeycodeFromString(PlayerPrefs.GetString(Setting.Jump.ToString()));
            _instance.jump = jump;
        }
        else
        {
            PlayerPrefs.SetString(Setting.Jump.ToString(), _instance.jump.ToString());
        }
        #endregion

        #region Sensitivity & Mouse
        //Sensitivity X
        if (PlayerPrefs.HasKey(Setting.SensX.ToString()))
        {
            _instance.sensitivity.x = PlayerPrefs.GetFloat(Setting.SensX.ToString());
        }
        else
        {
            PlayerPrefs.SetFloat(Setting.SensX.ToString(), _instance.sensitivity.x);
        }
        
        //Sensitivity Y
        if (PlayerPrefs.HasKey(Setting.SensY.ToString()))
        {
            _instance.sensitivity.y = PlayerPrefs.GetFloat(Setting.SensY.ToString());
        }
        else
        {
            PlayerPrefs.SetFloat(Setting.SensY.ToString(), _instance.sensitivity.y);
        }
        #endregion
    }

    public static KeyCode GetKeycodeFromString(string key)
    {
        KeyCode keyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), key);

        return keyCode;
    }
    #endregion
}
