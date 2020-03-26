using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class FireProjectile : MonoBehaviour
{

    [SerializeField] private Transform projectileTransform;
    [SerializeField] private float projectileFireSpeed = 30f;
    [SerializeField] private float minimumSecondsTilProjectileFire = 1.0f;

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
        if( mouse != null )
        {   //if left click and projectile ready
            if(mouse.leftButton.isPressed && secondsTilProjectileFire <= 0f )
            {
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
        //if gamepad connected
        if(gamepad != null){
            //if right trigger pressed and projectile ready
            if( gamepad.rightShoulder.isPressed && secondsTilProjectileFire <= 0f){
                //instantiate projectile, reset timer
                secondsTilProjectileFire = ShootProjectile(secondsTilProjectileFire);
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
    }


    //instantiates a projectile and sets it's velocity
    private float ShootProjectile(float secondsTilProjectileFire)
    {
        //reset timer
        secondsTilProjectileFire = minimumSecondsTilProjectileFire;
        //instantiate projectile
        Transform firedProjectileClone = Object.Instantiate(projectileTransform, this.transform.position, this.transform.rotation);
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
