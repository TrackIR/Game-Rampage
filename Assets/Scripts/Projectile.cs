using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
     // Basic Settings
     public float speed = 75f;
     public float lifeTime = 5f; // How long it lives before auto-destroying

     void Start()
     {
          // 1. Get the Rigidbody component
          Rigidbody rb = GetComponent<Rigidbody>();

          // 2. Make it move forward
          if (rb != null)
          {
               rb.linearVelocity = transform.forward * speed;
          }

          // 3. Set it to destroy itself after 'lifeTime' seconds
          Destroy(gameObject, lifeTime);
     }

     void OnCollisionEnter(Collision collision)
     {
          // TODO: Check if player was hit
          // if (collision.gameObject.CompareTag("Player"))
          // {
          //     // TODO: Add damage logic here
          // }

          // 4. Destroy the projectile on impact with anything (building or player)
          Destroy(gameObject);
     }
}