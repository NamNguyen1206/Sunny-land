using UnityEngine;
using System.Collections;

public class AttackState : BossState
{
    private float attackCooldown = 1.5f;
    private float timer;

    public AttackState(Boss boss) : base(boss) { }

    public override void Enter()
    {
        timer = 0;
        boss.animator.Play("d_cleave");
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer >= attackCooldown)
        {
            if (boss.DistanceToPlayer() > boss.attackRange)
                boss.stateMachine.ChangeState(boss.chaseState);
            else
                boss.animator.Play("d_cleave"); // đánh tiếp

            timer = 0;
        }
    }
}
