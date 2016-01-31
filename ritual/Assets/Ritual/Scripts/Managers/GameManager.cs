using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public Bird m_Bird;
    public Transform m_DancePos;

    public PatternRecognizer m_PatternRecognizer;

    public enum GamePhase
    {
        NOT_STARTED,
        ENTER,
        LOOK_AT,
        DANCE,
        WAIT,
        DISAPPOINT,
        APPROVE,
        LOSE,
        WIN,
    }
    protected GamePhase m_CurrentPhase = GamePhase.NOT_STARTED;

    public void Start()
    {
        if (m_PatternRecognizer == null)
        {
            m_PatternRecognizer = FindObjectOfType<PatternRecognizer>();
        }
    }

    public void StartGame()
    {
        m_CurrentPhase = GamePhase.ENTER;
    }

    public void Update()
    {
        switch (m_CurrentPhase)
        {
            case GamePhase.ENTER:
                HandleEnterPhase();
                break;
            case GamePhase.LOOK_AT:
                HandleLookAtPhase();
                break;
            case GamePhase.DANCE:
                HandleDancePhase();
                break;
            case GamePhase.WAIT:
                HandleWaitPhase();
                break;
            case GamePhase.DISAPPOINT:
                HandleDisappointPhase();
                break;
            case GamePhase.APPROVE:
                HandleApprovePhase();
                break;
            case GamePhase.LOSE:
                HandleLosePhase();
                break;
            case GamePhase.WIN:
                HandleWinPhase();
                break;
        }
    }

    public void HandleEnterPhase()
    {
        if (m_Bird.MoveTo(m_DancePos.position))
        {
            Debug.Log("Transitioning to look at: " + m_CurrentPhase.ToString());
            m_CurrentPhase = GamePhase.LOOK_AT;
        }
    }

    public void HandleLookAtPhase()
    {
        Debug.Log("Looking");
        if (m_Bird.LookDir(m_DancePos.forward))
        {
            Debug.Log("Transitioing to Dance");
            m_CurrentPhase = GamePhase.DANCE;
        }
    }

    public void HandleDancePhase()
    {
        m_CurrentPhase = GamePhase.WAIT;
    }

    public void HandleWaitPhase()
    {
        
    }

    public void HandleDisappointPhase()
    {
        m_CurrentPhase = GamePhase.DANCE;
    }

    public void HandleApprovePhase()
    {
        m_CurrentPhase = GamePhase.DANCE;
    }

    public void HandleLosePhase()
    {

    }

    public void HandleWinPhase()
    {

    }
}
