public class BringerOfDeathStateMachine
{
    public BringerOfDeathState currentState;

    public void ChangeState(BringerOfDeathState newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}
