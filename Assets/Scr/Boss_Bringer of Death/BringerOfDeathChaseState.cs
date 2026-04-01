using UnityEngine;

public class BringerOfDeathChaseState : BringerOfDeathState
{
    public BringerOfDeathChaseState(BringerOfDeathBoss boss) : base(boss) { }

    public override void Enter()
    {
        Debug.Log("Bringer Of Death: Enter CHASE");
        boss.PlayWalkAnimation();
    }

    public override void Update()
    {
        float distanceToPlayer = boss.DistanceToPlayer();

        // if (boss.DistanceToPlayer() > boss.chaseLoseRange)
        // {
        //     boss.stateMachine.ChangeState(boss.patrolState);
        //     return;
        // }
        //
        // boss.MoveToPlayer();
        //
        // if (boss.DistanceToPlayer() <= boss.attackRange)
        // {
        //     boss.stateMachine.ChangeState(boss.attackState);
        // }

        if (distanceToPlayer > boss.chaseLoseRange)
        {
            boss.stateMachine.ChangeState(boss.patrolState);
            return;
        }

        if (distanceToPlayer <= boss.attackRange)
        {
            boss.stateMachine.ChangeState(boss.attackState);
            return;
        }

        if (distanceToPlayer <= boss.chaseRange)
        {
            boss.stateMachine.ChangeState(boss.castState);
            return;
        }

        boss.MoveToPlayer();
    }
}
