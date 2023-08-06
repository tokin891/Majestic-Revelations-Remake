using UnityEngine;

public sealed class Weapon : MonoBehaviour
{
    [Header("Details")]
    [SerializeField] WeaponDetails indexWeapon;
    [SerializeField] InteractRaycast raycastInfo;
    [Tooltip("Lock weapon for a couple of seconds when you enable it")]
    [SerializeField] float lockOnEnable;
    [SerializeField] float giveDamageEnemy;

    [Header("Input")]
    [SerializeField] KeyCode shotKey;
    [SerializeField] TypeShot typeShot;

    [Header("Object")]
    [SerializeField] ParticleSystem muzzle;
    [SerializeField] AudioSource shotSource;

    [Header("Events")]
    [SerializeField] UnityEngine.Events.UnityEvent onShot;

    private float nextTiemToShot;
    private float unlockOnEnable;

    private void Awake()
    {
        shotSource.clip = indexWeapon.ShotSound;
    }
    private void Update()
    {
        if (unlockOnEnable > Time.time)
            return;
        if (Cursor.lockState == CursorLockMode.None)
            return;

        if(typeShot == TypeShot.Tap)
        {
            if(Input.GetKeyDown(shotKey) && nextTiemToShot < Time.time)
            {
                Shot();
            }
        }
        if(typeShot == TypeShot.Hold)
        {
            if (Input.GetKey(shotKey) && nextTiemToShot < Time.time)
            {
                Shot();
            }
        }
    }

    private void Shot()
    {
        RaycastHit hit = raycastInfo.GetRaycastObject(indexWeapon.Range);

        if (hit.transform.GetComponent<NPC>() != null)
            return;

        nextTiemToShot = Time.time + indexWeapon.DelayToNexyShot;

        if (muzzle != null)
            muzzle.Play();
        onShot?.Invoke();
        shotSource.Play();

        if (hit.transform == null)
            return;

        Debug.Log(hit.transform.name);
        ImpactManager.Instance.CreateImpact(hit);
        if(hit.transform.TryGetComponent(out Rigidbody body))
        {
            body.AddForce(-hit.normal * 1000f, ForceMode.Force);
        }
        if(hit.transform.TryGetComponent(out ObjectDestroyable objectDestroy))
        {
            objectDestroy.TakeDamage(0f, true);
        }
        if(hit.transform.TryGetComponent(out EnemyHitbox hb))
        {
            hb.TakeDamage(new Damage{
                damage = giveDamageEnemy
            });
        }
    }

    public enum TypeShot
    {
        Tap,
        Hold
    }

    private void OnEnable()
    {
        unlockOnEnable = Time.time + lockOnEnable;
    }
}
