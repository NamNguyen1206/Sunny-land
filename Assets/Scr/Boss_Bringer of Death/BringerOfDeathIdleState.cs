public class BringerOfDeathIdleState : BringerOfDeathState
{
    public BringerOfDeathIdleState(BringerOfDeathBoss boss) : base(boss) { }

    public override void Enter()
    {
        boss.StopMoving();
        boss.PlayIdleAnimation();
    }

    public override void Update()
    {
        if (boss.DistanceToPlayer() < boss.chaseRange)
        {
            boss.stateMachine.ChangeState(boss.chaseState);
        }
    }
}
