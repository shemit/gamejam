using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public Bird m_Bird;
    public Transform m_DancePos;

    public PatternRecognizer m_PatternRecognizer;
    public Transform m_FeedbackVFXSpawnPos;

    public GameObject m_ApprovalFeedbackVFX;
    public GameObject m_DisappointFeedbackVFX;

    public int m_MaxTries = 3;
    protected int m_NumTriesLeft = 0;

    public int m_NumDancesToWin = 5;
    protected int m_NumDancesCompleted = 0;

    public float m_TimeBetweenPreviewMove = 0.5f;
    public float m_TimePerPlayerMove = 2.0f;

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

    protected float m_WaitTimer = 0.0f;
    protected float m_DanceTimeLimitInSeconds = 0.0f;

    protected int m_DanceStepIndex = 0;
    protected float m_DanceStepTimer = 0.0f;

    public void Start()
    {
        if (m_PatternRecognizer == null)
        {
            m_PatternRecognizer = FindObjectOfType<PatternRecognizer>();
        }
    }

    public void StartGame()
    {
        m_NumTriesLeft = m_MaxTries;
        m_NumDancesCompleted = 0;
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
            m_CurrentPhase = GamePhase.LOOK_AT;
        }
    }

    public void HandleLookAtPhase()
    {
        if (m_Bird.LookDir(m_DancePos.forward))
        {
            m_PatternRecognizer.Init(m_PatternRecognizer.LastPatternPrefab);
            TransitionToDance();
        }
    }

    public void TransitionToDance()
    {
        m_DanceStepIndex = 0;
        m_DanceStepTimer = 0.0f;
        // TODO: play first dance step
        m_CurrentPhase = GamePhase.DANCE;
    }

    public void HandleDancePhase()
    {
        m_DanceStepTimer += Time.deltaTime;
        if (m_DanceStepTimer >= m_TimeBetweenPreviewMove)
        {
            m_DanceStepIndex++;
            if (m_DanceStepIndex >= m_PatternRecognizer.ActivePattern.m_PatternSequence.Length)
            {
                m_PatternRecognizer.ActivePattern.ResetSequence();
                m_WaitTimer = 0.0f;
                m_DanceTimeLimitInSeconds = m_PatternRecognizer.ActivePattern.m_PatternSequence.Length * m_TimePerPlayerMove;
                m_CurrentPhase = GamePhase.WAIT;
            }
            else
            {
                // TODO: play next dance step
                m_TimeBetweenPreviewMove = 0.0f;
            }
        }
    }

    public void HandleWaitPhase()
    {
        m_WaitTimer += Time.deltaTime;
        if (m_WaitTimer > m_DanceTimeLimitInSeconds)
        {
            m_NumTriesLeft--;
            if (m_NumTriesLeft <= 0)
            {
                m_CurrentPhase = GamePhase.LOSE;
            }
            else
            {
                m_CurrentPhase = GamePhase.DISAPPOINT;
            }
        }
        else if (m_PatternRecognizer.ActivePattern.IsComplete)
        {
            m_NumDancesCompleted++;
            if (m_NumDancesCompleted >= m_NumDancesToWin)
            {
                m_CurrentPhase = GamePhase.WIN;
            }
            else
            {
                m_CurrentPhase = GamePhase.APPROVE;
            }
        }
    }

    public void HandleDisappointPhase()
    {
        
        m_PatternRecognizer.ActivePattern.ResetSequence();
        TransitionToDance();
    }

    public void HandleApprovePhase()
    {
        
        m_PatternRecognizer.InitRandomPattern();
        TransitionToDance();
    }

    public void HandleLosePhase()
    {

    }

    public void HandleWinPhase()
    {

    }

    public void SpawnFeedbackVFX(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, m_FeedbackVFXSpawnPos.position, m_FeedbackVFXSpawnPos.rotation) as GameObject;
        obj.transform.SetParent(m_FeedbackVFXSpawnPos);
    }
}
