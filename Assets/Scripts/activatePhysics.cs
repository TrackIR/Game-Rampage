using UnityEngine;



public class activatePhysics : MonoBehaviour
{
    private Rigidbody rb;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Decor")
        {
            rb = collision.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddForce(transform.forward * 7f, ForceMode.Impulse);
            print("Collision detected with player, gravity activated.");

            Destroy(collision.gameObject, 5f);
        }
    }
}
