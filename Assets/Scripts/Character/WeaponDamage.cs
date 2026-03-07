using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponDamage : MonoBehaviour
{
    [SerializeField]
    private int damage = 10;

    [Header("Owner/Input")]
    [SerializeField]
    private string ownerTag = "Player1";
    [SerializeField]
    private GameObject ownerObject;
    [SerializeField]
    private float inputPressThreshold = 0.5f;

    [Header("Collider")]
    [SerializeField]
    private Collider damageCollider;

    private bool hasHitThisSwing = false;

    private void Awake()
    {
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        if (damageCollider != null)
        {
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        EnsureOwnerTag();
    }

    private void Start()
    {
        EnsureOwnerTag();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageCollider == null || !damageCollider.enabled || hasHitThisSwing)
        {
            return;
        }

        EnsureOwnerTag();
        if (string.IsNullOrEmpty(ownerTag) || ownerTag == "Untagged")
        {
            return;
        }   

        var target = other.GetComponentInParent<PlayerMovement>();
        if (target == null)
        {
            return;
        }

        if (target.CompareTag(ownerTag))
        {
            return;
        }

        hasHitThisSwing = true;
        target.TakeDamage(damage, ownerTag);
        Debug.Log($"{ownerTag} hit {target.name} for {damage} damage.");
    }

    public void EnableHitBox()
    {
        if (damageCollider == null)
        {
            return;
        }

        if (damageCollider.enabled)
        {
            return;
        }

        EnsureOwnerTag();
        hasHitThisSwing = false;
        damageCollider.enabled = true;
    }

    public void DisableHitBox()
    {
        if (damageCollider == null)
        {
            return;
        }

        damageCollider.enabled = false;
    }

    private void EnsureOwnerTag()
    {
        if (ownerObject == null)
        {
            var ownerMovement = GetComponentInParent<PlayerMovement>();
            if (ownerMovement != null)
            {
                ownerObject = ownerMovement.gameObject;
            }
            else
            {
                ownerObject = transform.root.gameObject;
            }
        }

        if (ownerObject != null && !string.IsNullOrEmpty(ownerObject.tag) && ownerObject.tag != "Untagged")
        {
            ownerTag = ownerObject.tag;
        }
    }
}