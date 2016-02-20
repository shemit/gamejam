using UnityEngine;
using System.Collections;

public class Pattern : MonoBehaviour {
    public int m_RequiredCompletionCount = 1;
    public Collider[] m_PatternSequence;

    private int[] m_SequenceTracker;
    private GameObject[] m_Indicators;

    private int m_CurrentSequenceIndex = -1;
    private bool m_IsComplete;
    public bool IsComplete { get { return m_IsComplete; } }
    private Collider m_LastHitCollider = null;

    public bool m_IndicatorsAreSetup = false;

    public enum SequenceAdvancementResult
    {
        NO_MATCH = 0,
        NEW_SEQUENCE_STARTED,
        SEQUENCE_ADVANCED,
        COMPLETE,
        HIT_LAST_COLLIDER,
        WRONG_HIT,
    }

    public void OnDrawGizmos()
    {
        if (m_PatternSequence != null && m_PatternSequence.Length > 0)
        {
            for (int i = 0, n = m_PatternSequence.Length-1; i < n; ++i)
            {
                if (m_PatternSequence[i] != null && m_PatternSequence[i + 1] != null)
                {
                    Gizmos.color = i == 0?Color.green:Color.red;
                    Gizmos.DrawLine(m_PatternSequence[i].transform.position, m_PatternSequence[i + 1].transform.position);
                }
            }
        }
    }

    public void SetupIndicators(GameObject indicatorPrefab)
    {
        if (m_IndicatorsAreSetup)
        {
            return;
        }

        m_Indicators = new GameObject[m_PatternSequence.Length];
        for (int i = 0, n = m_PatternSequence.Length; i < n; ++i)
        {
            Renderer[] renderers = m_PatternSequence[i].GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
            {
                Destroy(r);//.enabled = false;
            }

            Transform trans = m_PatternSequence[i].transform;
            GameObject indicatorObj = Instantiate(indicatorPrefab, trans.position, trans.rotation) as GameObject;
            indicatorObj.transform.SetParent(trans);
        }
        m_IndicatorsAreSetup = true;
    }

    public void Init()
    {
        m_SequenceTracker = new int[m_PatternSequence.Length];
        for (int i = 0, n = m_SequenceTracker.Length; i < n; ++i)
        {
            m_SequenceTracker[i] = 0;
        }
    }

    public void Hide()
    {
        m_PatternSequence[m_CurrentSequenceIndex].gameObject.SetActive(false);
    }

    public void Show()
    {
        m_PatternSequence[m_CurrentSequenceIndex].gameObject.SetActive(true);
    }

    public SequenceAdvancementResult AdvanceSequence(Collider hitCollider)
    {
        //if (hitCollider == m_LastHitCollider)
            //return SequenceAdvancementResult.HIT_LAST_COLLIDER;

        SequenceAdvancementResult result = SequenceAdvancementResult.NO_MATCH;

        // sequence already in progress so check for next in sequence
        if (hitCollider == m_PatternSequence[m_CurrentSequenceIndex])
        {
            SetCurrentSequenceIndex(GetNextIndex(m_CurrentSequenceIndex));
            result = SequenceAdvancementResult.SEQUENCE_ADVANCED;
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

        m_PatternSequence[m_CurrentSequenceIndex].gameObject.SetActive(false);
        m_LastHitCollider = m_PatternSequence[m_CurrentSequenceIndex];
        m_SequenceTracker[m_CurrentSequenceIndex]++;

        m_CurrentSequenceIndex = i;

        // update object for feedback
        m_PatternSequence[m_CurrentSequenceIndex].gameObject.SetActive(true);
        Utils.SetColor(m_PatternSequence[m_CurrentSequenceIndex].gameObject, Color.green);

        // highlight next
        Utils.SetColor(m_PatternSequence[GetNextIndex(m_CurrentSequenceIndex)].gameObject, Color.yellow);
    }

    public void ResetSequence()
    {
        for(int i = 0, n = m_PatternSequence.Length; i < n; ++i)
        {
			if (m_PatternSequence [i] == null) 
			{
				continue;
			}
            m_PatternSequence[i].gameObject.SetActive(false);
            Utils.SetColor(m_PatternSequence[i].gameObject, Color.white);
            m_SequenceTracker[i] = 0;
        }

        m_PatternSequence[0].gameObject.SetActive(true);
        m_LastHitCollider = null;
        m_CurrentSequenceIndex = 0;
        m_IsComplete = false;
    }

    public void SetSequenceColor(Color c)
    {
        for (int i = 0, n = m_PatternSequence.Length; i < n; ++i)
        {
            m_PatternSequence[i].gameObject.SetActive(true);
            Utils.SetColor(m_PatternSequence[i].gameObject, c);
        }
    }
}
