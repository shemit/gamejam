using UnityEngine;
using System.Collections;

public class Pattern : MonoBehaviour {
    public int m_RequiredCompletionCount = 1;
    public Collider[] m_PatternSequence;

    private int[] m_SequenceTracker;

    private int m_CurrentSequenceIndex = -1;
    private bool m_IsComplete;
    public bool IsComplete { get { return m_IsComplete; } }
    private Collider m_LastHitCollider = null;

    public enum SequenceAdvancementResult
    {
        NO_MATCH = 0,
        NEW_SEQUENCE_STARTED,
        SEQUENCE_ADVANCED,
        COMPLETE,
        HIT_LAST_COLLIDER,
        WRONG_HIT,
    }

    public void Init()
    {
        m_SequenceTracker = new int[m_PatternSequence.Length];
        for (int i = 0, n = m_SequenceTracker.Length; i < n; ++i)
        {
            m_SequenceTracker[i] = 0;
        }
    }

    public SequenceAdvancementResult AdvanceSequence(Collider hitCollider)
    {
        if (hitCollider == m_LastHitCollider)
            return SequenceAdvancementResult.HIT_LAST_COLLIDER;

        SequenceAdvancementResult result = SequenceAdvancementResult.NO_MATCH;

        if (m_CurrentSequenceIndex == -1)
        {
            // sequence hasn't started so use this first hit as the start
            for (int i = 0, n = m_PatternSequence.Length; i < n; ++i)
            {
                if (m_PatternSequence[i] == hitCollider)
                {
                    SetCurrentSequenceIndex(i);
                    result = SequenceAdvancementResult.NEW_SEQUENCE_STARTED;
                    break;
                }
            }
        }
        else
        {
            // sequence already in progress so check for next in sequence
            int nextIndex = GetNextIndex(m_CurrentSequenceIndex);
            if (hitCollider == m_PatternSequence[nextIndex])
            {
                SetCurrentSequenceIndex(nextIndex);
                result = SequenceAdvancementResult.SEQUENCE_ADVANCED;
            }
        }

        // check for completion
        m_IsComplete = true;
        for (int i = 0, n = m_SequenceTracker.Length; i < n; ++i)
        {
            if (m_SequenceTracker[i] < m_RequiredCompletionCount)
            {
                m_IsComplete = false;
                break; // sequence was not completed
            }
        }

        if(m_IsComplete)
        {
            result = SequenceAdvancementResult.COMPLETE;
        }

        return result;
    }

    public int GetNextIndex(int i)
    {
        return (i + 1) % m_PatternSequence.Length;
    }

    public void SetCurrentSequenceIndex(int i)
    {
        if(i < 0 || i >= m_PatternSequence.Length)
        {
            return;
        }

        m_CurrentSequenceIndex = i;

        // update object for feedback
        m_LastHitCollider = m_PatternSequence[m_CurrentSequenceIndex];
        m_SequenceTracker[m_CurrentSequenceIndex]++;
        Utils.SetColor(m_PatternSequence[m_CurrentSequenceIndex].gameObject, Color.green);

        // highlight next
        Utils.SetColor(m_PatternSequence[GetNextIndex(m_CurrentSequenceIndex)].gameObject, Color.yellow);
    }

    public void ResetSequence()
    {
        for(int i = 0, n = m_PatternSequence.Length; i < n; ++i)
        {
            Utils.SetColor(m_PatternSequence[i].gameObject, Color.white);
            m_SequenceTracker[i] = 0;
        }
        m_LastHitCollider = null;
        m_CurrentSequenceIndex = -1;
        m_IsComplete = false;
    }

    

    public void SetSequenceColor(Color c)
    {
        for (int i = 0, n = m_PatternSequence.Length; i < n; ++i)
        {
            Utils.SetColor(m_PatternSequence[i].gameObject, c);
        }
    }
}
