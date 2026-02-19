using UnityEngine;

public class UltimateLaser : MonoBehaviour
{
    public int damage = 100;
    public float duration = 3f;
    public LayerMask targetLayers;

    public float startRotationX = 10f;
    public float endRotationX = 90f;

    private float elapsedTime = 0f;

    private Quaternion startLocalRotation;
    private Quaternion endLocalRotation;

    private Transform parentTransform;

    private void Start()
    {
        parentTransform = transform.parent;

        // 1. Read Y scale value
        float yScale = transform.localScale.y;

        // 2. Move yScale/2 in Y axis
        transform.localPosition = new Vector3(0f, yScale / 2f, 0f);

        // 3. Rotate parent empty
        startLocalRotation = Quaternion.Euler(startRotationX, 0f, 0f);
        endLocalRotation = Quaternion.Euler(endRotationX, 0f, 0f);

        parentTransform.localRotation = startLocalRotation;

        Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (parentTransform == null) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);

        parentTransform.localRotation =
            Quaternion.Lerp(startLocalRotation, endLocalRotation, t);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            BuildingDestruction building = other.GetComponent<BuildingDestruction>();
            if (building != null)
                building.TakeDamage();
        }
    }
}
