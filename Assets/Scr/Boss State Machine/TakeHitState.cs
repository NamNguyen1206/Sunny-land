using UnityEngine;

public class TakeHitState : BossState
{
    private float timer = 0.5f;
    private float duration = 0.5f; // Thời gian chờ cố định

    public TakeHitState(Boss boss) : base(boss) { }

    public override void Enter()
    
    {
        timer = duration;
        boss.animator.Play("d_take_hit", -1, 0f);
        boss.rb.linearVelocity = Vector2.zero;
        Debug.Log("Boss: Ouch! (Entering TakeHit)");
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
