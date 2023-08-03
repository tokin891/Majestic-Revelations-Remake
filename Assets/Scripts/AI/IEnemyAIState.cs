public interface IEnemyAIState
{
    void EnterState(EnemyAI ai);
    void UpdateState();
    void ExitState();
}
