using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyController : MonoBehaviour
{   /*  INPUT  */
    private Gamepad gamepad = null;
    private Keyboard keyboard = null;
    /*  Camera  */
    [SerializeField] private Camera cam = null;
    /*  Colliders  */
    [SerializeField] new private Collider collider;
    /*  PUBLIC FIELDS  */
    [SerializeField] public float deadzoneXMouse = 0f;
    [SerializeField] public float deadzoneYMouse = 0f;
    [SerializeField] public float deadzoneXGamepadLeftStick = 0.05f;
    [SerializeField] public float deadzoneYGamepadLeftStick = 0.05f;
    /*  PRIVATE CONTROL RELATED  */
    private Vector2 movementInput;
    [SerializeField] private readonly float moveSpeed = 5f;
    [SerializeField] private readonly float maxInclineAngle = 45f;
    [SerializeField] private readonly float climbSpeed = 2f;
    [SerializeField] private readonly float jumpSpeed = 15.0f;
    [SerializeField] private readonly float fallSpeedMultiplier = 1.3f;
    [SerializeField] private readonly float groundedGravityMultiplier = 0f;
    [SerializeField] private readonly float rotationalSpeed = 500f;
    /*  Trick Related  */
    [SerializeField] private readonly float flipSpeed = 500f;
    [SerializeField] private readonly float spinSpeed = 500f;
    [SerializeField] private readonly float cleanAngle = 30f;
    [SerializeField] private readonly float normalAngle = 50f;
    [SerializeField] private readonly float sloppyAngle = 80f;
    [SerializeField] private readonly float tripVelocityMagnitude = 1f;
    /*  Upkeep Variables  */
    private bool isGrounded = false;
    private Vector3 groundNormal = Vector3.zero;
    private Vector3 groundNormalProjection = Vector3.zero;
    private float distanceToRotateNormalToGround = 10f;
    private float hitDistance = 0f;
    private bool groundIsBelow = false;
    private bool isClimbing = false;
    private Vector3 gravity = Vector3.zero;
    private ContactPoint[] contactPoints= null;
    private Collision currentCollision = null;
    //CONSTANTS
    Rigidbody rb = null;
    private float colliderRadius = 0f;

    private void Awake(){
        keyboard = Keyboard.current;
        gamepad = Gamepad.current;  
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        colliderRadius = (collider.bounds.max.x - collider.bounds.min.x) / 2f;
    }

    void Update()
    {
        movementInput = CheckMovementKeyboard(keyboard);
        if(gamepad != null && movementInput == Vector2.zero){ movementInput = CheckMovementLeftStick(gamepad); }
        if( rb.velocity != Vector3.zero && !isGrounded)
        {
            //Rotate according to keyboard input if in air
            Vector2 rotVelocity = CheckRotationKeyboard(keyboard);
            //if no keyboard input
            if( gamepad != null && rotVelocity == Vector2.zero ) { rotVelocity = gamepad.dpad.ReadValue(); }
            //if rotating
            if( rotVelocity != Vector2.zero )
            {
                transform.Rotate(rotVelocity.y * flipSpeed * Time.deltaTime, rotVelocity.x * spinSpeed * Time.deltaTime, 0);
            }
            else if( hitDistance < distanceToRotateNormalToGround ) {
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(new Vector3(rb.velocity.x, 0f, rb.velocity.z)), rotationalSpeed * Time.deltaTime);
                //this.transform.localRotation = Quaternion.RotateTowards(this.transform.localRotation, Quaternion.LookRotation(new Vector3(groundNormal.x - 270 < 0 ? groundNormal.x+90:groundNormal.x-270, 0f, groundNormal.z)), rotationalSpeed * Time.deltaTime);

            }
            else
            {
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(new Vector3(rb.velocity.x, 0f, rb.velocity.z)), rotationalSpeed * Time.deltaTime);
            }
        }
        if (isGrounded)
        {
            Vector3 forwardNormalVector = Vector3.ProjectOnPlane(cam.transform.forward.normalized, Vector3.up).normalized;
            Vector3 rightNormalVector = Vector3.ProjectOnPlane(cam.transform.right.normalized, Vector3.up).normalized;
            Vector3 combinedVector = new Vector3(
                    forwardNormalVector.x * movementInput.y +
                    rightNormalVector.x * movementInput.x,
                    0,
                    forwardNormalVector.z * movementInput.y +
                    rightNormalVector.z * movementInput.x);
            rb.velocity = Vector3.ProjectOnPlane(new Vector3(combinedVector.x * moveSpeed, rb.velocity.y, combinedVector.z * moveSpeed), groundNormal);
            //Automatically rotate towards direrction of velocity
            if (rb.velocity != Vector3.zero){
                this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(rb.velocity), rotationalSpeed * Time.deltaTime);
            }
            //Handle jumping
            if ( JumpButtonIsPressed(keyboard, gamepad) ){
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
            }
        }
        //if not grounded
        else
        {
            Ray downwardRay = new Ray(new Vector3(transform.position.x, collider.bounds.min.y, transform.position.z), Vector3.down);
            groundIsBelow = Physics.Raycast(downwardRay, out RaycastHit hit);
            groundNormalProjection = hit.normal;
            hitDistance = hit.distance;
            //if falling
            if (rb.velocity.y < 0)
            {   //make fall faster
                rb.velocity += Vector3.up * gravity.y * fallSpeedMultiplier * Time.deltaTime;
            }
            
        }
    }
    void FixedUpdate()
    {
        ApplyGravity();
    }

    void ApplyGravity()
    {
        if( isClimbing ) {          gravity = Vector3.zero;    }
        else if( !isGrounded ) {    gravity = Physics.gravity; }
        else {
            gravity = -groundNormal * gravity.magnitude * groundedGravityMultiplier;
        }
        rb.AddForce(gravity, ForceMode.Force);

    }
    private Vector2 CheckMovementKeyboard(Keyboard keyboard){
        Vector2 movementInput = Vector2.zero;
        if( keyboard.wKey.isPressed ) { movementInput.y += 1f; }
        if( keyboard.sKey.isPressed ) { movementInput.y -= 1f; }
        if( keyboard.dKey.isPressed ) { movementInput.x += 1f; }
        if( keyboard.aKey.isPressed ) { movementInput.x -= 1f; }
        return movementInput;
    }

    private Vector2 CheckRotationKeyboard(Keyboard keyboard) {
        Vector2 rotVelocity = Vector2.zero;
        if( keyboard.upArrowKey.isPressed       ) { rotVelocity.y += 1f; }
        if( keyboard.downArrowKey.isPressed     ) { rotVelocity.y -= 1f; }
        if( keyboard.rightArrowKey.isPressed    ) { rotVelocity.x += 1f; }
        if( keyboard.leftArrowKey.isPressed     ) { rotVelocity.x -= 1f; }
        return rotVelocity;
    }

    private Vector2 CheckMovementLeftStick(Gamepad gamepad) {
        movementInput = gamepad.leftStick.ReadValue();
        if( Mathf.Abs(movementInput.x) < deadzoneXGamepadLeftStick ) { movementInput.x = 0f; }
        if( Mathf.Abs(movementInput.y) < deadzoneYGamepadLeftStick ) { movementInput.y = 0f; }
        return movementInput;
    }

    private bool JumpButtonIsPressed(Keyboard keyboard, Gamepad gamepad) {

        return keyboard.spaceKey.isPressed || (gamepad != null && (gamepad.aButton.isPressed || gamepad.xButton.isPressed));
    }

    private bool CheckGrounded(Collision collision) {
   
        contactPoints = new ContactPoint[collision.contactCount];
        collision.GetContacts(contactPoints);
        
        foreach (ContactPoint contactPoint in contactPoints) {
            if (maxInclineAngle > Vector3.Angle(contactPoint.normal, -Physics.gravity.normalized)) {
                groundNormal = contactPoint.normal;
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentCollision = collision;
        isGrounded = CheckGrounded(collision);

        if (collision.gameObject.CompareTag("Climbable")) {
            isClimbing = true;
        }
        if (isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            //if no movement input
            if (movementInput.x == 0f && movementInput.y == 0f) {
                rb.velocity = Vector3.zero;
            }
            //angle between character rotation and normal to surface of collision
            float angleDif = 0f;
            Vector3 collisionAngleVector;
            //if not moving
            if (rb.velocity == Vector3.zero || rb.velocity.magnitude < tripVelocityMagnitude) {
                collisionAngleVector = new Vector3(Quaternion.LookRotation(groundNormal).eulerAngles.x, transform.rotation.eulerAngles.y, /*transform.rotation.z*/Quaternion.LookRotation(groundNormal).eulerAngles.z);
                angleDif = Quaternion.Angle(Quaternion.LookRotation(new Vector3((Quaternion.LookRotation(groundNormal).eulerAngles.x - 270f < 0) ? (Quaternion.LookRotation(groundNormal).eulerAngles.x + 90f) : (Quaternion.LookRotation(groundNormal).eulerAngles.x - 270f), transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Vector3.up), Quaternion.LookRotation(transform.rotation.eulerAngles, Vector3.up));

            }
            else {
                collisionAngleVector = new Vector3(rb.velocity.x, Quaternion.LookRotation(groundNormal, Vector3.up).eulerAngles.y, rb.velocity.z);
                angleDif = Mathf.Abs(Mathf.Abs(Quaternion.Angle(Quaternion.LookRotation(new Vector3(rb.velocity.x, Quaternion.LookRotation(groundNormal, Vector3.up).eulerAngles.y, rb.velocity.z), Vector3.up), transform.rotation)));
            }

            print("collisionAngleVector: " + collisionAngleVector);
            print("transform rotation: " + transform.rotation.eulerAngles);
            print("angle dif: " + angleDif);
            if ((angleDif < cleanAngle || 360 - angleDif < cleanAngle))
            {
                Debug.Log("Clean Landing");
            }
            else if ((angleDif < normalAngle || 360 - angleDif < normalAngle))
            {
                Debug.Log("Normal Landing");
            }
            else if (angleDif < sloppyAngle || 360 - angleDif < sloppyAngle)
            {
                Debug.Log("Sloppy Landing");
            }
            else
            {
                Debug.Log("Failed Landing");
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = CheckGrounded(collision);
        if (!isGrounded && collision.gameObject.CompareTag("Climbable")){
            Vector3 upNormalVector = Vector3.ProjectOnPlane(rb.transform.up, Vector3.forward).normalized;
            Vector3 collDir = collision.contacts[0].point - transform.position;
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(new Vector3(collDir.x, 0f, collDir.z)), rotationalSpeed * Time.deltaTime);

            Vector3 forwardNormalVector = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            Vector3 forwardNormalVectorXZ = new Vector3(forwardNormalVector.x, 0f, forwardNormalVector.z).normalized;
            Vector3 rightNormalVector = Vector3.ProjectOnPlane(cam.transform.right, Vector3.up).normalized;
            Vector3 forwardNormalVectorCam = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up).normalized;
            
            Vector3 forward = collision.contacts[0].normal;
            Vector3 back = -forward;
            Vector3 right = new Vector3(collision.contacts[0].normal.z, collision.contacts[0].normal.y, -collision.contacts[0].normal.x);
            Vector3 left = -right;
            Vector3 up = new Vector3(collision.contacts[0].normal.z, collision.contacts[0].normal.x, collision.contacts[0].normal.y).normalized;

            Vector3 down = -up;
            rb.velocity = new Vector3((back.x * movementInput.y +
                    left.x * movementInput.x) * climbSpeed,

                    Mathf.Abs(forwardNormalVectorXZ.x + forwardNormalVectorXZ.z) * movementInput.y * climbSpeed/* * (forwardNormalVector.x < 0f ? 1f : -1f) * (forwardNormalVector.z > 0f ? 1f : -1f)*//* +
                    Mathf.Abs(((forwardNormalVectorCam.x + forwardNormalVectorCam.z) * Input.GetAxis("Horizontal")))*/
                    , (back.z * movementInput.y +
                    left.z * movementInput.x) * climbSpeed);
            //Vector3 right = new Vector3(collision.contacts[0].normal.z, collision.contacts[0].normal.y, -collision.contacts[0].normal.x);
            if (rb.velocity.x > collision.contacts[0].normal.z){ 
                     
            }
        }
    }


    private void OnCollisionExit(Collision collision)
    {
        currentCollision = null;
        print("Collision Exit: " + collision);
        isGrounded = CheckGrounded(collision);

        
        if (collision.gameObject.CompareTag("Climbable"))
        {
       
            isClimbing = false;
        }
    }

    private bool CheckClimbing(Collision collision) {
        contactPoints = new ContactPoint[collision.contactCount];
        collision.GetContacts(contactPoints);
        foreach (ContactPoint contactPoint in contactPoints)
        {
            if (contactPoint.otherCollider.gameObject.CompareTag("Climbable"))
            {

                return true;
            }
        }
        return false;
    }
}
/////////////////////////////////////////////////////////////////////////////
/// DEPRACATED
/////////////////////////////////////////////////////////////////////////////
/*

        //[SerializeField] private Camera playerCam = null;


        //private Vector2 movementInput;
    //private Vector2 lookInput;

    //private PlayerInput input;
     //private Collision curCollision = null;
     //private float vaultSpeed = .2f;

    //private float dragCoeff = 0.3f;
     //private float sqrtCollRadius = 0f;

    //private float tinyDistance = 1f;

    //public float lowJumpMultiplier = 100f;

    //input.PlayerControls.Move.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
    //Cursor.lockState = CursorLockMode.Locked;

     /*
        if (isClimbing) {
            
            if (Input.GetButtonDown("Jump"))
            {
                Vector3 upNormalVector = Vector3.ProjectOnPlane(rb.transform.up, Vector3.forward).normalized;
                
                Vector3 forwardNormalVector = Vector3.ProjectOnPlane(rb.transform.forward, Vector3.up).normalized;
                Vector3 rightNormalVector = Vector3.ProjectOnPlane(rb.transform.right, Vector3.up).normalized;
                Vector3 forwardNormalVectorCam = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
                rb.AddForce(new Vector3((forwardNormalVector.x * movementInput.y +
                        rightNormalVector.x * movementInput.x)*100f,
                        jumpSpeed
                        , (forwardNormalVector.z * movementInput.x +
                        rightNormalVector.z * movementInput.y)*100f), ForceMode.VelocityChange);
            }
             
        }
    
//print(movementInput.x + movementInput.y);

//rb.velocity = new Vector3(rb.velocity.x * (1 - dragCoeff), rb.velocity.y, rb.velocity.z * (1 - dragCoeff));

            //Vector3 velocityXZ = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

    //print("x: "+Mathf.Abs(transform.rotation.eulerAngles.x - collision.contacts[0].normal.x));
            //print("velocity angle: " + Quaternion.LookRotation(rb.velocity).eulerAngles);
            //print("character angle: " + transform.rotation.eulerAngles);
           //print("z: " + Mathf.Abs(transform.rotation.eulerAngles.z - collision.contacts[0].normal.z));
            //float xAdd = Mathf.Abs(transform.rotation.eulerAngles.x + Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up).eulerAngles.x);
            //print("local rot: " + transform.localRotation.eulerAngles.x + " Global rot:" + transform.rotation.eulerAngles.x);

                
            //float xDif = Mathf.Abs(transform.rotation.eulerAngles.x - Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up).eulerAngles.x);
            //print(transform.rotation.eulerAngles.x + " this x" + Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up).eulerAngles.x);
            //print(transform.rotation.eulerAngles.z + " this z" + Quaternion.LookRotation(collision.contacts[0].normal, Vector3.up).eulerAngles.z);

                //print("velocity: " + rb.velocity);
            //print("normal3: " + collision.contacts[0].normal);

     //collisionAngleVector = new Vector3(Quaternion.LookRotation(collision.contacts[0].normal).eulerAngles.x, transform.rotation.eulerAngles.y, Quaternion.LookRotation(collision.contacts[0].normal).eulerAngles.z);
               

    //print((forwardNormalVector.x + forwardNormalVector.z) * movementInput.y);
            rb.velocity = new Vector3((forwardNormalVector.x * movementInput.y +
                    rightNormalVector.x * movementInput.x),

                    Mathf.Abs(forwardNormalVector.x+forwardNormalVector.z) * movementInput.y * verticalClimbSpeed/* * (forwardNormalVector.x < 0f ? 1f : -1f) * (forwardNormalVector.z > 0f ? 1f : -1f)*//* +
                    Mathf.Abs(((forwardNormalVectorCam.x + forwardNormalVectorCam.z) * Input.GetAxis("Horizontal")))
                    , (forwardNormalVector.z * movementInput.y +
                    rightNormalVector.z * movementInput.x));
            
    //makes getting up slopes slower have no idea why this works
        if( isGrounded ) {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 2f, rb.velocity.z );
        }
*/
