using UnityEngine;

public class BringerOfDeathCastState : BringerOfDeathState
{
    private float timer;
    private bool spellSpawned;

    public BringerOfDeathCastState(BringerOfDeathBoss boss) : base(boss) { }

    public override void Enter()
    {
        timer = 0f;
        spellSpawned = false;
        boss.StopMoving();
        boss.FacePlayer();
        boss.PlayCastAnimation();
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        boss.StopMoving();
        boss.FacePlayer();

        if (!spellSpawned && timer >= boss.spellSpawnDelay)
        {
            spellSpawned = true;
            boss.SpawnSpellAttack();
        }

        if (timer >= boss.spellCastDuration)
        {
            if (boss.DistanceToPlayer() <= boss.attackRange)
            {
                boss.stateMachine.ChangeState(boss.attackState);
            }
            else if (boss.DistanceToPlayer() > boss.chaseLoseRange)
            {
                boss.stateMachine.ChangeState(boss.patrolState);
            }
            else
            {
                boss.stateMachine.ChangeState(boss.chaseState);
            }
        }
    }
}
