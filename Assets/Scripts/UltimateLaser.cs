using UnityEngine;

public class UltimateLaser : MonoBehaviour
{
    public float damagePerSecond = 100f;
    public float duration = 3f;
    public LayerMask targetLayers;
    //public GameObject highlights;

    public float laserSpin = 360f;
    //public float highlightSpin = -360f;

    private float elapsedTime = 0f;
    private Transform parentTransform;

    private void Start()
    {
        parentTransform = transform.parent;

        // keep pivot offset logic
        float zScale = transform.localScale.z;
        transform.localPosition = new Vector3(0f, 0f, zScale / 2f);

        Destroy(parentTransform.gameObject, duration);
    }

    private void Update()
    {
        if (parentTransform == null) return;

        elapsedTime += Time.deltaTime;

        // lock to main cam forward
        //Transform cam = Camera.main.transform;
        //parentTransform.rotation = Quaternion.LookRotation(cam.forward);

        Spin(gameObject, laserSpin);
        //if (highlights != null)
        //    Spin(highlights, highlightSpin);
    }

    private void Spin(GameObject obj, float speed)
    {
        obj.transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) == 0)
        {
            return;
        }

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(Mathf.RoundToInt(damagePerSecond * Time.deltaTime));
        }

        BuildingDestruction building = other.GetComponent<BuildingDestruction>();
        if (building != null)
        {
            building.TakeDamage();
        }
    }
}