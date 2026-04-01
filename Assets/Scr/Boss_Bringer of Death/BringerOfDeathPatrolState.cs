public class BringerOfDeathPatrolState : BringerOfDeathState
{
    public BringerOfDeathPatrolState(BringerOfDeathBoss boss) : base(boss) { }

    public override void Enter()
    {
        boss.PlayWalkAnimation();
    }

    public override void Update()
    {
        boss.Patrol();

        if (boss.DistanceToPlayer() < boss.chaseRange)
        {
            boss.stateMachine.ChangeState(boss.chaseState);
        }
    }
}
