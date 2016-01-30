using UnityEngine;
using System.Collections;

public class PatternRecognizer : MonoBehaviour {
    private static int m_CachedPatternMask = 0;
    public static int PatternMask
    {
        get
        {
            if(m_CachedPatternMask == 0)
            {
                m_CachedPatternMask = LayerMask.GetMask("Pattern");
            }
            return m_CachedPatternMask;
        }
    }

    public Pattern m_ActivePattern;
    public Transform m_HeadTransform;
    public float m_SphereCastRadius = 1.0f;
    public float m_MaxTimeBeforeSequenceAdvancement = 1.0f;

    public Pattern[] m_Patterns;

    private float m_TimeSinceLastAdvancement = 0;

    public void Init(Pattern pattern)
    {
        if(m_ActivePattern != null)
        {
            m_ActivePattern.gameObject.SetActive(false);
        }

        m_ActivePattern = pattern;
        m_ActivePattern.Init();
        m_ActivePattern.ResetSequence();
        m_ActivePattern.gameObject.SetActive(true);
    }

    public void Start()
    {
        Init(m_ActivePattern);
    }

    public void Update()
    {
        if(m_ActivePattern.IsComplete)
        {
            return;
        }

        m_TimeSinceLastAdvancement += Time.deltaTime;

        if(m_TimeSinceLastAdvancement > m_MaxTimeBeforeSequenceAdvancement)
        {
            m_ActivePattern.ResetSequence();
        }

        RaycastHit hit;
        Ray headRay = new Ray(m_HeadTransform.position, m_HeadTransform.forward);
        //if(Physics.Raycast(headRay, out hit, float.MaxValue, PatternMask))
        if(Physics.SphereCast(headRay, m_SphereCastRadius, out hit, float.MaxValue, PatternMask))
        {
            if(m_ActivePattern != null)
            {
                Pattern.SequenceAdvancementResult result = m_ActivePattern.AdvanceSequence(hit.collider);
                switch(result)
                {
                    case Pattern.SequenceAdvancementResult.NEW_SEQUENCE_STARTED:
                    case Pattern.SequenceAdvancementResult.SEQUENCE_ADVANCED:
                        m_TimeSinceLastAdvancement = 0;
                        break;
                    case Pattern.SequenceAdvancementResult.COMPLETE:
                        m_TimeSinceLastAdvancement = 0;
                        StartCoroutine(DoWinSequence(1.0f));
                        break;
                    case Pattern.SequenceAdvancementResult.NO_MATCH:
                        m_ActivePattern.ResetSequence();
                        break;
                }
                Debug.Log(hit.collider.name);
            }
        }
    }

    public IEnumerator DoWinSequence(float duration)
    {
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            m_ActivePattern.SetSequenceColor(new Color(Random.value + 0.5f, Random.value + 0.5f, Random.value + 0.5f));
            yield return new WaitForEndOfFrame();
        }

        //m_ActivePattern.ResetSequence();
        PickRandomPattern();
    }

    public void PickRandomPattern()
    {
        Init(m_Patterns[Random.Range(0, m_Patterns.Length)]);
    }
    
}
