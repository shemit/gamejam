using UnityEngine;
using System.Collections;

public class IntroManager : MonoBehaviour {
    public Bird m_MaleBird;
    public Bird m_FemaleBird;

    public Transform m_MaleBirdDancePos;
    public Transform m_FemaleBirdDancePos;
    public Transform m_FinalPos;

    public enum IntroPhase
    {
        NOT_STARTED,
        ENTER,
        LOOK_AT,
        DANCE1,
        LEAVE,
        MATING,
        FINISH,
    }
    protected IntroPhase m_CurrentPhase = IntroPhase.NOT_STARTED;

    [ContextMenu("Start Intro")]
    public void StartIntro()
    {
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
            case IntroPhase.LEAVE:
                HandleLeavePhase();
                break;
        }
    }

    public void HandleEnterPhase()
    {
        bool maleAtPosition = m_MaleBird.MoveTo(m_MaleBirdDancePos.position);
        bool femaleAtPosition = m_FemaleBird.MoveTo(m_FemaleBirdDancePos.position);
        if (maleAtPosition && femaleAtPosition)
        {
            m_CurrentPhase = IntroPhase.LOOK_AT;
        }
    }

    public void HandleLookAtPhase()
    {
        bool maleCorrectFacing = m_MaleBird.LookDir(m_MaleBirdDancePos.forward);
        bool femaleCorrectFacing = m_FemaleBird.LookDir(m_FemaleBirdDancePos.forward);
        if (maleCorrectFacing && femaleCorrectFacing)
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
}
