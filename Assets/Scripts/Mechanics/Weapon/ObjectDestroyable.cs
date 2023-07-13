using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestroyable : MonoBehaviour
{
    [Range(1f,100f)]
    [SerializeField] private float health;
    [SerializeField] GameObject respawnOnDestroy;
    [SerializeField] GameObject[] randomSpawnObject;
    [SerializeField] Vector3 respPoint;
    [Header("Events")]
    [Space]
    [SerializeField] UnityEngine.Events.UnityEvent onDestroyObject;

    public void TakeDamage(float damage = 0, bool randomDamage = false)
    {
        if(randomDamage)
        {
            health -= Random.Range(1f, 55f);
        }else
        {
            health -= damage;
        }

        if(health <= 0)
        {
            // Destroy
            if (respawnOnDestroy != null)
                Instantiate(respawnOnDestroy, transform.position, transform.rotation);
            onDestroyObject?.Invoke();
            Instantiate(randomSpawnObject[Random.Range(0,randomSpawnObject.Length)], transform.position + respPoint, Quaternion.identity);
            if (respawnOnDestroy != null)
                Destroy(gameObject);
        }
    }
    public void ChangeMaterail(Material selectMaterial)
    {
        GetComponent<Renderer>().material = selectMaterial;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + respPoint, 0.25f) ;
    }
}
