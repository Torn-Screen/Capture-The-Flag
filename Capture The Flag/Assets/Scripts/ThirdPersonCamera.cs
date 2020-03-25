using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform lookAt;
    public Transform camTransform;

    [SerializeField]
    private Camera cam;

    Mouse mouse;
    Gamepad gamepad = null;
    UnityEngine.InputSystem.Utilities.ReadOnlyArray<UnityEngine.InputSystem.Gamepad> gamepads;

    //private Vector2 lookInput;
    //private CameraControls input;

    private const float Y_ANGLE_MIN = 0.0f;
    private const float Y_ANGLE_MAX = 60.0f;

    public float camDistance;
    private float currX = 0.0f;
    private float currY = 0.0f;
    public float deadzoneX = 0.05f;
    public float deadzoneY = 0.05f;

    public float sensitivityX = 1.0f;
    public float sensitivityY = 1.0f;

    public float sensitivityXGamepadRightStick = 100f;
    public float sensitivityYGamepadRightStick = 100f;
    public float deadzoneXGamepadRightStick = 0.05f;
    public float deadzoneYGamepadRightStick = 0.05f;

    private void Awake()
    {
        mouse = Mouse.current;
        gamepad = Gamepad.current;
        gamepads = Gamepad.all;
    }

        private void Start()
    {
        camDistance = cam.transform.position.z - lookAt.position.z;
        

    }

    private void Update()
    {
        float mouseX = mouse.delta.x.ReadValue();
        float mouseY = mouse.delta.y.ReadValue();
        float gamepadRightStickX = 0f;
        float gamepadRightStickY = 0f;
        bool lookingX = false;
        bool lookingY = false;
        if (gamepad != null)
        {
            gamepadRightStickX = gamepad.rightStick.ReadValue().x;
            gamepadRightStickY = gamepad.rightStick.ReadValue().y;
            lookingX = (Mathf.Abs(gamepadRightStickX) > deadzoneXGamepadRightStick);
            lookingY = (Mathf.Abs(gamepadRightStickY) > deadzoneYGamepadRightStick);
        }
        if (mouseX == 0 && mouseY == 0 && (lookingX || lookingY))
        {
            if (lookingX)
            {
                currX += gamepadRightStickX * sensitivityXGamepadRightStick * Time.deltaTime;
            }
            if (lookingY)
            {
                currY -= gamepadRightStickY * sensitivityYGamepadRightStick * Time.deltaTime;
            }
        }
        
        else
        {
            currX += mouseX * sensitivityX * Time.deltaTime;
            currY -= mouseY * sensitivityY * Time.deltaTime;

        }
        currY = Mathf.Clamp(currY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, camDistance);

        Quaternion rotation = Quaternion.Euler(currY, currX, 0);
        camTransform.position = lookAt.position + rotation * dir;
        camTransform.LookAt(lookAt.position);
    }
}
