using UnityEngine;

public class BringerOfDeathDeathState : BringerOfDeathState
{
    public BringerOfDeathDeathState(BringerOfDeathBoss boss) : base(boss) { }

    public override void Enter()
    {
        boss.PlayDeathAnimation();
        boss.StopMoving();

        if (boss.rb != null)
        {
            boss.rb.bodyType = RigidbodyType2D.Static;
        }

        Collider2D bossCollider = boss.GetComponent<Collider2D>();
        if (bossCollider != null)
        {
            bossCollider.enabled = false;
        }

        // Neu sau nay ban muon roi chest bang animation event, giu lai ham SpawnChest().
        Object.Destroy(boss.gameObject, 3f);
    }
}
