public class EnemyAI 
{
    private IEnemyAIState currentState;

    public void SetState(IEnemyAIState state)
    {
        if (currentState != null)
            currentState.ExitState();

        currentState = state;
        currentState.EnterState(this);
    }

    public void Update()
    {
        if (currentState != null)
            currentState.UpdateState();
    }
}


