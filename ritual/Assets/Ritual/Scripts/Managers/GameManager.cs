using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    public Bird m_Bird;
    public Transform m_DancePos;
    public Transform m_LosePos;
    public Transform m_WinPos;

    public PatternRecognizer m_PatternRecognizer;
    public Transform m_FeedbackVFXSpawnPos;

    public GameObject m_ApprovalFeedbackVFX;
    public GameObject m_DisappointFeedbackVFX;

    public HealthIndicator m_HealthIndicator;
    public int m_MaxTries = 3;
    protected int m_NumTriesLeft = 0;

    protected int m_NumDancesToWin = 0;
    protected int m_NumDancesCompleted = 0;

    public Transform m_StepPreviousHeadReferencePos;
    public float m_StepPreviewDanceDistanceScaler = 4.0f;
    public float m_TimeBetweenPreviewMove = 0.5f;
    public float m_TimePerPlayerMove = 2.0f;
    public float m_MinTimeToComplete = 10.0f;

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

    public void Start()
    {
        if (m_PatternRecognizer == null)
        {
            m_PatternRecognizer = FindObjectOfType<PatternRecognizer>();
        }
        m_NumDancesToWin = m_PatternRecognizer.m_PatternPrefabs.Count;

        if (m_HealthIndicator == null)
        {
            m_HealthIndicator = FindObjectOfType<HealthIndicator>();
        }
        m_HealthIndicator.Init(m_MaxTries);
    }

    public void StartGame()
    {
        m_NumTriesLeft = m_MaxTries;
        m_NumDancesCompleted = 0;
        m_PatternRecognizer.PauseSequenceRecognition();
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

    [ContextMenu("Transition to Dance Phase")]
    public void TransitionToDance()
    {
        m_PatternRecognizer.PauseSequenceRecognition();
        m_Bird.Dance(m_PatternRecognizer.ActivePattern);
        m_CurrentPhase = GamePhase.DANCE;
    }

    public void HandleDancePhase()
    {
        if (m_Bird.DanceCompleted)
        {
            m_WaitTimer = 0.0f;
            m_DanceTimeLimitInSeconds = Mathf.Max(m_MinTimeToComplete, m_PatternRecognizer.ActivePattern.m_PatternSequence.Length * m_TimePerPlayerMove);
            m_PatternRecognizer.ActivePattern.ResetSequence();
            m_PatternRecognizer.ResumeSequenceRecognition();
            m_CurrentPhase = GamePhase.WAIT;
        }
    }

    public void HandleWaitPhase()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            TransitionToDance();
            return;
        }

        m_WaitTimer += Time.deltaTime;
        if (m_WaitTimer > m_DanceTimeLimitInSeconds || Input.GetKeyDown(KeyCode.F))
        {
            m_PatternRecognizer.PauseSequenceRecognition();
            m_NumTriesLeft--;
            m_HealthIndicator.DeductHP();
            SpawnFeedbackVFX(m_DisappointFeedbackVFX);
            if (m_NumTriesLeft <= 0)
            {
                m_Bird.TriggerLoseAnimation();
                m_Bird.m_MaxTurnDegreesPerSecond *= 3;
                m_CurrentPhase = GamePhase.LOSE;
            }
            else
            {
                m_Bird.TriggerDisappointAnimation();
                m_CurrentPhase = GamePhase.DISAPPOINT;
            }
        }
        else if (m_PatternRecognizer.ActivePattern.IsComplete || Input.GetKeyDown(KeyCode.C))
        {
            m_PatternRecognizer.PauseSequenceRecognition();
            m_NumDancesCompleted++;
            for (int i = 0; i < m_NumDancesCompleted; ++i)
            {
                SpawnFeedbackVFX(m_ApprovalFeedbackVFX);
            }
            if (m_NumDancesCompleted >= m_NumDancesToWin)
            {
                m_Bird.TriggerWinAnimation();
                m_CurrentPhase = GamePhase.WIN;
            }
            else
            {
                m_Bird.TriggerApproveAnimation();
                m_CurrentPhase = GamePhase.APPROVE;
            }
        }
    }

    public void HandleDisappointPhase()
    {
        if (!m_Bird.IsPlayingAnimation())
        {
            m_PatternRecognizer.ActivePattern.ResetSequence();
            TransitionToDance();
        }
    }

    public void HandleApprovePhase()
    {
        if (!m_Bird.IsPlayingAnimation())
        {
            //m_PatternRecognizer.InitRandomPattern();
            m_PatternRecognizer.InitNextPattern();
            TransitionToDance();
        }
    }

    public void HandleLosePhase()
    {
        if (!m_Bird.IsPlayingAnimation())
        {
            if (m_Bird.MoveTo(m_LosePos.position))
            {
                // restart
                Application.LoadLevel(0);
            }
        }
    }

    public void HandleWinPhase()
    {
        if (!m_Bird.IsPlayingAnimation())
        {
            if (m_Bird.MoveTo(m_WinPos.position))
            {
                // restart
                Application.LoadLevel(0);
            }
        }
    }

    public void SpawnFeedbackVFX(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, m_FeedbackVFXSpawnPos.position, m_FeedbackVFXSpawnPos.rotation) as GameObject;
        obj.transform.SetParent(m_FeedbackVFXSpawnPos);
    }
}
