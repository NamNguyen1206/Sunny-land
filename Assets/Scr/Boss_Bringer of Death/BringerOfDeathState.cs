public abstract class BringerOfDeathState
{
    protected BringerOfDeathBoss boss;

    protected BringerOfDeathState(BringerOfDeathBoss boss)
    {
        this.boss = boss;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
