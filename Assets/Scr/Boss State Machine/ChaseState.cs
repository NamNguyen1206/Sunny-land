using UnityEngine;

public class ChaseState : BossState
{
    public ChaseState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log("Enter CHASE");
        boss.animator.Play("d_walk");
    }

    public override void Update()
    {
        boss.MoveToPlayer();

        if (boss.DistanceToPlayer() <= boss.attackRange)
        {
            boss.stateMachine.ChangeState(boss.attackState);
        }
    }
}
