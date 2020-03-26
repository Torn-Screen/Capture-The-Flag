using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ThirdPersonCamera : MonoBehaviour
{
    //Location of camera
    public Transform camTransform;
    //player location
    public Transform lookAt;
    //input
    Mouse mouse = null;
    Gamepad gamepad = null;
    //camera angle bounds
    [SerializeField] private const float Y_ANGLE_MIN = 0.0f;
    [SerializeField] private const float Y_ANGLE_MAX = 60.0f;
    //distance from camera to player
    [SerializeField] private float camDistance = 0f;
    //current camera X angle
    private float currX = 0.0f;
    //current camera Y angle
    private float currY = 0.0f;
    [SerializeField] public float deadzoneX = 0.05f;
    [SerializeField] public float deadzoneY = 0.05f;
    //mouse sensitivity
    [SerializeField] public float sensitivityX = 1.0f;
    [SerializeField] public float sensitivityY = 1.0f;
    //gamepad parameters
    [SerializeField] public float sensitivityXGamepadRightStick = 100f;
    [SerializeField] public float sensitivityYGamepadRightStick = 100f;
    [SerializeField] public float deadzoneXGamepadRightStick = 0.05f;
    [SerializeField] public float deadzoneYGamepadRightStick = 0.05f;

    private void Awake()
    {   //get input mechanisms
        mouse = Mouse.current;
        gamepad = Gamepad.current;
    }

    private void Start()
    {
        //calculate distance of  camera to player
        camDistance = this.transform.position.z - lookAt.position.z;
    }

    private void Update()
    {
        //get mouse delta
        float mouseX = mouse.delta.x.ReadValue();
        float mouseY = mouse.delta.y.ReadValue();
        //if no mouse input and gamepad is connected
        if (mouseX == 0 && mouseY == 0 && gamepad != null)
        {
            //get gamepad right stick position
            Vector2 gamepadRightStickPosition = getGamepadRightStickPosition(gamepad);

            currX += gamepadRightStickPosition.x * sensitivityXGamepadRightStick * Time.deltaTime;
            currY -= gamepadRightStickPosition.y * sensitivityYGamepadRightStick * Time.deltaTime;
        }
        //if mouse input or gamepad not connected
        else
        {
            currX += mouseX * sensitivityX * Time.deltaTime;
            currY -= mouseY * sensitivityY * Time.deltaTime;
        }
        //clamp camera Y angle
        currY = Mathf.Clamp(currY, Y_ANGLE_MIN, Y_ANGLE_MAX);
    }

    private void LateUpdate()
    {
        //point camera at character
        Vector3 dir = new Vector3(0, 0, camDistance);
        Quaternion rotation = Quaternion.Euler(currY, currX, 0);
        this.transform.position = lookAt.position + rotation * dir;
        this.transform.LookAt(lookAt.position);
    }

    private Vector2 getGamepadRightStickPosition(Gamepad gamepad) {
        //get right stick input
        float gamepadRightStickX = gamepad.rightStick.ReadValue().x;
        float gamepadRightStickY = gamepad.rightStick.ReadValue().y;
        
        //check if less than deadzone
        if( Mathf.Abs(gamepadRightStickX) < deadzoneXGamepadRightStick )
        {
            gamepadRightStickX = 0f;
        }
        if( Mathf.Abs(gamepadRightStickY) < deadzoneYGamepadRightStick )
        {
            gamepadRightStickY = 0f;
        }
        return new Vector2(gamepadRightStickX, gamepadRightStickY);
    }
}
