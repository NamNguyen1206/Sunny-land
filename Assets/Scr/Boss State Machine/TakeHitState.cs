using UnityEngine;

public class TakeHitState : BossState
{
    private float timer = 0.5f;

    public TakeHitState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.animator.Play("d_take_hit");
        boss.rb.linearVelocity = Vector2.zero;
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            boss.stateMachine.ChangeState(boss.chaseState);
        }
    }
}
