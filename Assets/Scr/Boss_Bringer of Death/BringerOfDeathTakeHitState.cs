using UnityEngine;

public class BringerOfDeathTakeHitState : BringerOfDeathState
{
    private float timer = 0.5f;
    private float duration = 0.5f;

    public BringerOfDeathTakeHitState(BringerOfDeathBoss boss) : base(boss) { }

    public override void Enter()
    {
        timer = duration;
        boss.PlayHurtAnimation();
        boss.StopMoving();
        Debug.Log("Bringer Of Death: Ouch! (Entering TakeHit)");
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (boss.DistanceToPlayer() <= boss.chaseRange)
            {
                boss.stateMachine.ChangeState(boss.chaseState);
            }
            else
            {
                boss.stateMachine.ChangeState(boss.patrolState);
            }
        }
    }
}
