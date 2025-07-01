using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    public delegate void OnHitSomething();

    public OnHitSomething OnHit;


    public float Damage = 10f;
    public LayerMask TargetLayerMask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision successful");
        if (!((TargetLayerMask.value & (1 << other.gameObject.layer)) > 0))
            return;

        Health targetHealth = other.gameObject.GetComponent<Health>();

        if (targetHealth == null)
            return;

        TryDamage(targetHealth);

    }

    private void TryDamage(Health targetHealth)
    {
        targetHealth.Damage(Damage, transform.parent.gameObject);
        Debug.Log("hit " + targetHealth);
        OnHit?.Invoke();
    }
}
