using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// This class manages the firing of a projectile 
/// Apply this class to the transform of 
/// The projectile spawn position
public class FireProjectile : MonoBehaviour
{

    [SerializeField] private Transform projectileTransform;
    [SerializeField] private float projectileFireSpeed = 30f;
    [SerializeField] private float minimumSecondsTilProjectileFire = 1.0f;
    [SerializeField] private Rigidbody rb = null;

    private bool fireButtonPressedLastFrame = false;
    private bool mouseFired = false;
    private bool gamepadFired = false;


    private float secondsTilProjectileFire = 0f;
    
    //input variables
    private Mouse mouse;
    private Gamepad gamepad;

    // Start is called before the first frame update
    void Start()
    {
        //get input on start
        mouse = Mouse.current;
        //if mouse not connected
        if( mouse == null )
        {
            Debug.Log("No mouse connected");
        }
        else {
            print("Mouse connected");
        }
        gamepad = Gamepad.current;
        //if gamepad not connected
        if( gamepad == null )
        {
            Debug.Log("No gamepad connected");
        }
        else
        {
            print("Gamepad connected");
        }
    }

    // Update is called once per frame
    void Update()
    {

        //update number of seconds til next shot
        secondsTilProjectileFire -= Time.deltaTime;
        //if mouse connected
        if( mouse != null && !gamepadFired)
        {   //if left click and projectile ready
            if(mouse.leftButton.isPressed)
            {
                if( fireButtonPressedLastFrame ) {
                    while( -secondsTilProjectileFire % minimumSecondsTilProjectileFire >= 0 )
                    {
                        secondsTilProjectileFire = ShootProjectiles(secondsTilProjectileFire);
                    }
                }
                while(-secondsTilProjectileFire % minimumSecondsTilProjectileFire >= 0 ) {
                    secondsTilProjectileFire = ShootProjectile(secondsTilProjectileFire);
                }
                //instantiate projectile, reset timer
                secondsTilProjectileFire = ShootProjectile(secondsTilProjectileFire);
            }
        }
        //otherwise try to find the mouse
        else {
            mouse = Mouse.current;
            //if mouse connected
            if( mouse != null )
            {
                print("Mouse connected");
            }
        }
        gamepadFired = false;
        //if gamepad connected
        if(gamepad != null && !mouseFired){
            //if right trigger pressed and projectile ready
            if( gamepad.rightShoulder.isPressed){
                if( fireButtonPressedLastFrame )
                {
                    ShootProjectiles(secondsTilProjectileFire);
                }
                else {
                    ShootProjectile(secondsTilProjectileFire);
                }
                //instantiate projectile, reset timer
                secondsTilProjectileFire = ShootProjectile(secondsTilProjectileFire);
                gamepadFired = true;
            }

        }
        //otherwise try to find gamepad
        else{
            gamepad = Gamepad.current;
            if( gamepad != null )
            {
                print("Gamepad connected");
            }

        }
        mouseFired = false;
    }

    //instantiates a projectile and sets it's velocity
    private float ShootProjectiles(float secondsTilProjectileFire)
    {
        while( secondsTilProjectileFire <= 0f )
        {
            Vector3 predictedPoint = this.transform.position + this.rb.velocity * -secondsTilProjectileFire;
            Transform firedProjectileClone = Object.Instantiate(projectileTransform, predictedPoint, this.transform.rotation);
           
            /*
            //get each child of projectile
            foreach( Transform child in firedProjectileClone.transform )
            {
                //get projectile rigidbody
                Rigidbody projectileRb = child.GetComponent<Rigidbody>();
                if( projectileRb != null )
                {
                    //set projectile velocity
                    projectileRb.velocity = this.transform.forward * projectileFireSpeed;
                    //stop searching for rigidbody
                    break;
                    
                }
            }
            */
            secondsTilProjectileFire += minimumSecondsTilProjectileFire;
        }
        //return timer
        return secondsTilProjectileFire;
    }
    //instantiates a projectile and sets it's velocity
    private float ShootProjectile(float secondsTilProjectileFire)
    {
        //reset number of seconds til fire
        while( secondsTilProjectileFire <= 0 ) {
            secondsTilProjectileFire += minimumSecondsTilProjectileFire;
        }
        //fire at projectile predicted position
        Vector3 predictedPoint = this.transform.position + this.rb.velocity * (minimumSecondsTilProjectileFire - secondsTilProjectileFire);
        //instantiate projectile
        Transform firedProjectileClone = Object.Instantiate(projectileTransform, predictedPoint, this.transform.rotation);
        //get each child of projectile
        foreach( Transform child in firedProjectileClone.transform )
        {
            //get projectile rigidbody
            Rigidbody  projectileRb = child.GetComponent<Rigidbody>();
            if( projectileRb != null ) {
                //set projectile velocity
                projectileRb.velocity = this.transform.forward * projectileFireSpeed;
                //stop searching for rigidbody
                break;
            }
        }
        //return timer
        return secondsTilProjectileFire;
    }
}
