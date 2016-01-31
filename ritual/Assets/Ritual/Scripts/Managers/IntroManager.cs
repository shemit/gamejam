using UnityEngine;
using System.Collections;

public class IntroManager : MonoBehaviour {
    public GameManager m_GameManager;
    public PatternRecognizer m_PatternRecognizer;

    public Bird m_MaleBird;
    public Bird m_FemaleBird;

    public Transform m_MaleBirdDancePos;
    public Transform m_FemaleBirdDancePos;
    public Transform m_FinalPos;

    public bool m_AutoStart = true;

    public enum IntroPhase
    {
        NOT_STARTED,
        ENTER,
        LOOK_AT,
        FEMALE_DANCE,
        MALE_DANCE,
        LOVE,
        LEAVE,
        MATING,
        FINISH,
    }
    protected IntroPhase m_CurrentPhase = IntroPhase.NOT_STARTED;

    public void Start()
    {
        if (m_GameManager == null)
        {
            m_GameManager = FindObjectOfType<GameManager>();
        }

        if (m_PatternRecognizer == null)
        {
            m_PatternRecognizer = FindObjectOfType<PatternRecognizer>();
        }

        if (m_AutoStart)
        {
            StartIntro();
        }
    }

    [ContextMenu("Start Intro")]
    public void StartIntro()
    {
        //m_PatternRecognizer.Init(null);
        m_PatternRecognizer.PauseSequenceRecognition();
        m_CurrentPhase = IntroPhase.ENTER;
    }

    public void Update()
    {
        switch (m_CurrentPhase)
        {
            case IntroPhase.ENTER:
                HandleEnterPhase();
                break;
            case IntroPhase.LOOK_AT:
                HandleLookAtPhase();
                break;
            case IntroPhase.FEMALE_DANCE:
                HandleFemaleDancePhase();
                break;
            case IntroPhase.MALE_DANCE:
                HandleMaleDancePhase();
                break;
            case IntroPhase.LOVE:
                HandleLovePhase();
                break;
            case IntroPhase.LEAVE:
                HandleLeavePhase();
                break;
            case IntroPhase.MATING:
                HandleMatingPhase();
                break;
            case IntroPhase.FINISH:
                HandleFinishPhase();
                break;
        }
    }

    public void HandleEnterPhase()
    {
        bool maleAtPosition = m_MaleBird.MoveTo(m_MaleBirdDancePos.position);
        bool femaleAtPosition = m_FemaleBird.MoveTo(m_FemaleBirdDancePos.position);
        if (maleAtPosition && femaleAtPosition)
        {
            m_FemaleBird.m_MaxTurnDegreesPerSecond *= 2;
            m_MaleBird.m_MaxTurnDegreesPerSecond *= 3;
            m_CurrentPhase = IntroPhase.LOOK_AT;
        }
    }

    public void HandleLookAtPhase()
    {
        bool maleCorrectFacing = m_MaleBird.LookDir(m_MaleBirdDancePos.forward);
        bool femaleCorrectFacing = m_FemaleBird.LookDir(m_FemaleBirdDancePos.forward);
        if (maleCorrectFacing && femaleCorrectFacing)
        {
            m_FemaleBird.StopWalking();
            m_MaleBird.StopWalking();

            m_FemaleBird.Dance(m_PatternRecognizer.ActivePattern);
            m_CurrentPhase = IntroPhase.FEMALE_DANCE;
        }
    }

    public void HandleFemaleDancePhase()
    {
        if (m_FemaleBird.DanceCompleted)
        {
            m_MaleBird.Dance(m_PatternRecognizer.ActivePattern, true);
            m_CurrentPhase = IntroPhase.MALE_DANCE;
        }
    }

    public void HandleMaleDancePhase()
    {
        if (m_MaleBird.DanceCompleted)
        {
            m_GameManager.SpawnFeedbackVFX(m_GameManager.m_ApprovalFeedbackVFX);
            m_FemaleBird.TriggerApproveAnimation();
            m_MaleBird.TriggerApproveAnimation();
            m_CurrentPhase = IntroPhase.LOVE;
        }
    }

    public void HandleLovePhase()
    {
        if (!m_FemaleBird.IsPlayingAnimation() && !m_MaleBird.IsPlayingAnimation())
        {
            m_CurrentPhase = IntroPhase.LEAVE;
        }
    }

    public void HandleLeavePhase()
    {
        bool maleAtPosition = m_MaleBird.MoveTo(m_FinalPos.position);
        bool femaleAtPosition = m_FemaleBird.MoveTo(m_FinalPos.position);
        if (maleAtPosition && femaleAtPosition)
        {
            m_CurrentPhase = IntroPhase.MATING;
        }
    }

    public void HandleMatingPhase()
    {
        m_CurrentPhase = IntroPhase.FINISH;
    }

    public void HandleFinishPhase()
    {
        // destroy intro elements
        m_GameManager.StartGame();
        m_CurrentPhase = IntroPhase.NOT_STARTED;
    }
}
