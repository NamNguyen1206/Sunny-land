using UnityEngine;

public class BossStateMachine
{
    public BossState currentState;

    public void ChangeState(BossState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}
