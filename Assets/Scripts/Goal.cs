using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private Transform m_OtherSpawn;
    private int m_GoalID = 0;
    private int m_GoalValue = 1;

    protected void Start()
    {
        InitialiseScoreID();
    }

    private void InitialiseScoreID()
    {
        if (name.Contains("PlayerOne"))
        {
            m_GoalID = 1;
        }
        else
        {
            m_GoalID = 2;
        }
    }

    public void Score()
    {
        ScoreManager.Instance.UpdateScore(m_GoalID, m_GoalValue);
    }

    protected void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball)
        {
            Score();
            ball.ResetBall(m_OtherSpawn.position);
        }
    }
}