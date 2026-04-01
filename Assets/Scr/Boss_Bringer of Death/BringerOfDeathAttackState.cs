using UnityEngine;

public class BringerOfDeathAttackState : BringerOfDeathState
{
    private float attackCooldown = 1.5f;
    private float timer;

    public BringerOfDeathAttackState(BringerOfDeathBoss boss) : base(boss) { }

    public override void Enter()
    {
        timer = 0f;
        boss.StopMoving();
        boss.FacePlayer();
        boss.PlayAttackAnimation();
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        boss.FacePlayer();

        if (timer >= attackCooldown)
        {
            if (boss.DistanceToPlayer() > boss.attackRange)
            {
                if (boss.DistanceToPlayer() <= boss.chaseRange)
                {
                    boss.stateMachine.ChangeState(boss.castState);
                }
                else
                {
                    boss.stateMachine.ChangeState(boss.chaseState);
                }
            }
            else
            {
                boss.PlayAttackAnimation();
            }

            timer = 0f;
        }
    }
}
