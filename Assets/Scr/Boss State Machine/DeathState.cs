using UnityEngine;

public class DeathState : BossState
{
    public DeathState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.animator.Play("death");
        boss.rb.linearVelocity = Vector2.zero;

        boss.GetComponent<Collider2D>().enabled = false;
    }
}
