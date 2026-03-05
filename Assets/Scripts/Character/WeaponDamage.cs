using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponDamage : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private string ownerTag = "Player1";

    [SerializeField]
    private float hitResetTime = 0.5f;

    [SerializeField]
    private Collider damageCollider;

    private void Awake()
    {
        var owner = GetComponentInParent<PlayerMovement>();
        if (owner != null)
        {
            ownerTag = owner.gameObject.tag;
        }
        else
        {
            ownerTag = transform.root.tag;
        }
    }

    private void Update()
    {
        //if (Input.GetButtonDown(ownerTag + "_" + "Attack")){damageCollider.enabled = true;}
    }
    private void OnTriggerEnter(Collider other)
    {
        if (string.IsNullOrEmpty(ownerTag))
        {
            return;
        }

        var root = other.transform.root;
        int targetId = root.GetInstanceID();

        // Ignore self/owner and prevent multiple hits on the same target per swing
        if (root.CompareTag(ownerTag))
        {
            return;
        }

        

        if (root.TryGetComponent(out PlayerMovement target))
        {
            target.TakeDamage(damage, ownerTag);
            //damageCollider.enabled = false;
        }
    }
}