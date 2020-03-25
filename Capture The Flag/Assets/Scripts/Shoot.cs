using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Shoot : MonoBehaviour
{
    [SerializeField]
    private Transform bullet;
    private Transform shootLocation;
    private float shotSecRemaining = 0.5f;
    private float shootSpeed = 30f;
    Mouse mouse;
    // Start is called before the first frame update
    void Start()
    {
        
        shootLocation = null;
        mouse = Mouse.current;
        foreach (Transform child in this.transform) {
            if (child.name == "Shoot Location") {
                shootLocation = child;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        shotSecRemaining -= Time.deltaTime;
        if (mouse.leftButton.isPressed  && shotSecRemaining <=0f) {
            Transform clone;
            Rigidbody cloneRb;
            shotSecRemaining = 0.1f;
            clone =  Object.Instantiate(bullet, shootLocation.transform.position, shootLocation.rotation);
            cloneRb = clone.GetComponent<Rigidbody>();
          
            cloneRb.velocity = transform.forward * shootSpeed;
        }
    }

    
}
