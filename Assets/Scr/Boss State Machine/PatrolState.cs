using UnityEngine;

public class PatrolState : BossState
{
    public PatrolState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        boss.animator.Play("d_walk");
    }

    public override void Update()
    {
        boss.Patrol();

        // Nếu thấy player → đuổi
        if (boss.DistanceToPlayer() < boss.chaseRange)
        {
            boss.stateMachine.ChangeState(boss.chaseState);
        }
    }
}
