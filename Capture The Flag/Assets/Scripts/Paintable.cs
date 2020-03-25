using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paintable : MonoBehaviour
{

    [SerializeField]
    Object orangeTexture;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //print("normal2: "+collision.contacts[0].normal);
        if (collision.gameObject.name == "Bullet(Clone)")
        {
           
            Instantiate(orangeTexture, collision.gameObject.transform.position, Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal));
            Destroy(collision.gameObject, 0f);
        }
    }
}
