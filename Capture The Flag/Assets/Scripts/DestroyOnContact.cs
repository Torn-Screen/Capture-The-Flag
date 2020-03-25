using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Material orangeMaterial;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Object orangeTexture;
    Renderer render;
    private bool hitBool;
    void Start()
    {
        render = GetComponent<Renderer>();
        render.material = orangeMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        RaycastHit raycastHit;
        Ray ray = new Ray(transform.position, transform.rotation.eulerAngles);
        if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.tag == "Paintable") {
            Instantiate(orangeTexture, raycastHit.point, Quaternion.Euler(new Vector3(raycastHit.normal.x-90f, raycastHit.normal.y, raycastHit.normal.z)), null);
        }
        */


       hitBool = Physics.Raycast(transform.position, transform.forward, 0.01f, LayerMask.GetMask("Default"));
        if (hitBool) {
            //Instantiate(orangeTexture, transform.position, Quaternion.Euler(new Vector3(transform.forward.x, transform.forward.y, transform.forward.z)), null);

        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        //print("normal: "+collision.contacts[0].normal);
        //Instantiate(orangeTexture, collision.transform.position, Quaternion.LookRotation(new Vector3(collision.contacts[0].normal.x, collision.contacts[0].normal.y, collision.contacts[0].normal.z), transform.up),null);
        //Instantiate(orangeTexture, collision.transform.position, Quaternion.Euler(new Vector3(collision.gameObject.transform.forward.x, collision.gameObject.transform.forward.y, collision.gameObject.transform.forward.z)), null);

        if (collision.gameObject.CompareTag("Ground")) {
            render.material = redMaterial;
            Destroy(this.gameObject, 5f);
        }
        /*
        if (collision.gameObject.tag == "Paintable"){
            Destroy(this.gameObject, 0f);
            Instantiate(orangeTexture, collision.contacts[0].thisCollider.transform.position, Quaternion.Euler(new Vector3(collision.GetContact(0).normal.x - 90f, collision.GetContact(0).normal.y, collision.GetContact(0).normal.z)));
        }
        */
    }
}
