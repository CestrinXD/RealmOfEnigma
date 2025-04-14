using System;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    private Collider2D attackCollider;

    // void Awake()
    // {
    //     attackCollider = GetComponent<Collider2D>();
    // }

    // void OnEnable()
    // {
    //     // Cek semua overlap saat attack area diaktifkan
    //     Collider2D[] hits = new Collider2D[10];
    //     int count = Physics2D.OverlapCollider(attackCollider, new ContactFilter2D(), hits);

    //     for (int i = 0; i < count; i++)
    //     {
    //         OnTriggerEnter2D(hits[i]);
    //     }
    // }

  
    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("hit", collider);
        if (collider.CompareTag("Enemy"))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage, hitDirection);
            }
        }
        
    }

    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     Debug.Log("stay hit ");
    // }
}
