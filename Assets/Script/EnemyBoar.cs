using System.Collections;
using UnityEngine;

public class EnemyBoar : Enemy
{
    

    protected override void Start()
    {
        base.Start();
        ENEMY_HIT = "Boar_hit";
        ENEMY_IDLE = "Boar_idle";
        ENEMY_WALK = "Boar_walk";

        moveSpeed = 2f;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Patrol()
    {
        // Bisa tambah patrol logic unik di sini
        base.Patrol();
    }

    public override void OnHit(Vector2 hitDirection)
    {
        base.OnHit(hitDirection);
        // Tambah efek boar misal knockback
    }

    
}
