using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager m_Instance;
    public static ScoreManager Instance { get { return m_Instance; } }
    
    // Score
    [SerializeField]
    private Text m_ScorePlayerOneText;
    private int m_ScorePlayerOne;
    [SerializeField]
    private Text m_ScorePlayerTwoText;
    private int m_ScorePlayerTwo;

	protected void Awake()
    {
        InitialiseSingleton();
        UpdateUI();
	}
	
    private void InitialiseSingleton()
    {
        if (m_Instance != null && m_Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Another instance of " + GetType() + " has been destroyed.");
        }
        else
        {
            m_Instance = this;
            Debug.Log(m_Instance + " --> if null, there's a problem.");
        }
    }

    public void UpdateScore(int goalID, int value)
    {
        switch (goalID)
        {
            // 1 ups the score of player 2 as it is: "Goal n°1 has been entered, so player n°2 scores" ; 
            case 1:
                m_ScorePlayerTwo += value;
                break;
            case 2:
                m_ScorePlayerOne += value;
                break;
            default:
                Debug.LogError("An invalid value has been passed to in UpdateScore(), in the ScoreManager.");
                break;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        m_ScorePlayerOneText.text = "Player 1: " + m_ScorePlayerOne.ToString();
        m_ScorePlayerTwoText.text = m_ScorePlayerTwo.ToString() + " :Player 2";
    }
}