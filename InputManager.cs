using UnityEngine;

[AddComponentMenu("Input/Manager")]
public class InputManager : MonoBehaviour
{
   [Header("Movement")]
   public KeyCode forward = KeyCode.W;
   public KeyCode back = KeyCode.S;
   public KeyCode right = KeyCode.D;
   public KeyCode left = KeyCode.A;

   private static InputManager _instance;

   private void Awake()
   {
      _instance = this;
      DontDestroyOnLoad(gameObject);
   }

   public static Vector2 Direction()
   {
      float x = 0;
      float y = 0;
      if (Input.GetKey(_instance.forward)) y += 1;
      if (Input.GetKey(_instance.back)) y -= 1;

      if (Input.GetKey(_instance.right)) x += 1;
      if (Input.GetKey(_instance.left)) x -= 1;

      return new Vector2(x, y);
   }

   public static Vector3 CursorPosition()
   {
      return Input.mousePosition;
   }
}