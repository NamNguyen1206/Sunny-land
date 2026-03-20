using UnityEngine;

public class IdleState : BossState
{
    public IdleState(Boss boss) : base(boss) { }
    public override void Enter()
    {
        boss.animator.Play("d_idle");
    }

    public override void Update()
    {
        if (boss.DistanceToPlayer() < boss.chaseRange)
        {
            boss.stateMachine.ChangeState(boss.chaseState);
        }
    }
}
