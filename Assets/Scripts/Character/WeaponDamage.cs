using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponDamage : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private string ownerTag = "Player1";

    private readonly HashSet<int> hitTargets = new HashSet<int>();

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

    private void OnEnable()
    {
        hitTargets.Clear();
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
        if (root.CompareTag(ownerTag) || hitTargets.Contains(targetId))
        {
            return;
        }

        hitTargets.Add(targetId);

        if (root.TryGetComponent(out PlayerMovement target))
        {
            target.TakeDamage(damage, ownerTag);
        }
    }

    private void Reset()
    {
        Collider c = GetComponent<Collider>();
        if (c != null)
        {
            c.isTrigger = true;
        }
    }

    public void ClearHitCache()
    {
        hitTargets.Clear();
    }
}