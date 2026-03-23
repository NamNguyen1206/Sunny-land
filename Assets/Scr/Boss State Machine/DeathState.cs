using UnityEngine;

public class DeathState : BossState
{
    public DeathState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.animator.Play("death");
        boss.rb.linearVelocity = Vector2.zero;
        boss.rb.bodyType = RigidbodyType2D.Static;
        if (boss.GetComponent<Collider2D>() != null)
        {
            boss.GetComponent<Collider2D>().enabled = false;
        }
        Object.Destroy(boss.gameObject, 3f);
    }
}
