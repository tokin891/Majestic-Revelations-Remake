using System.Collections;
using System.Collections.Generic;
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

        if (hit.transform.GetComponent<NPC>())
            return;

        nextTiemToShot = Time.time + indexWeapon.DelayToNexyShot;

        if (muzzle != null)
            muzzle.Play();
        onShot?.Invoke();
        shotSource.Play();

        if (hit.transform == null)
            return;

        // Shot Script
        ImpactManager.Instance.CreateImpact(hit);
        if(hit.transform.GetComponent<Rigidbody>())
        {
            hit.transform.GetComponent<Rigidbody>().AddForce(-hit.normal * 1000f, ForceMode.Force);
        }
        if(hit.transform.GetComponent<ObjectDestroyable>())
        {
            hit.transform.GetComponent<ObjectDestroyable>().TakeDamage(0f, true);
        }
        if(hit.transform.GetComponent<EnemyHitbox>())
        {
            hit.transform.GetComponent<EnemyHitbox>().TakeDamage(new Damage{ damage = giveDamageEnemy});
            Debug.Log("Shot " + hit.transform.name);
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
