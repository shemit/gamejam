using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    private static int m_CachedGazeUIMask = 0;
    public static int GazeUIMask
    {
        get
        {
            if (m_CachedGazeUIMask == 0)
            {
                m_CachedGazeUIMask = LayerMask.GetMask("GazeUI");
            }
            return m_CachedGazeUIMask;
        }
    }


    public Transform m_HeadTransform;
    public Transform m_PatternSpawnPos;
    public float m_SphereCastRadius = 1.0f;
    public float m_MaxTimeBeforeSequenceAdvancement = 1.0f;

    public bool m_AutoSelectNextPattern = true;

    public List<GameObject> m_PatternPrefabs;
    protected List<GameObject> m_UsedPatternPrefabs = new List<GameObject>();

    protected Pattern m_ActivePattern;
    public Pattern ActivePattern { get { return m_ActivePattern; } }
    protected GameObject m_LastPatternPrefab;
    public GameObject LastPatternPrefab { get { return m_LastPatternPrefab; } }

    private float m_TimeSinceLastAdvancement = 0;

    public void Init(GameObject patternPrefab)
    {
        StopCoroutine("DoWinSequence");
        if(m_ActivePattern != null)
        {
            Destroy(m_ActivePattern.gameObject);
        }

        if (patternPrefab != null)
        {
            m_LastPatternPrefab = patternPrefab;
            GameObject patternObj = Instantiate(patternPrefab, m_PatternSpawnPos.position, m_PatternSpawnPos.rotation) as GameObject;
            patternObj.transform.SetParent(m_PatternSpawnPos);
            m_ActivePattern = patternObj.GetComponent<Pattern>();
            m_ActivePattern.Init();
            m_ActivePattern.ResetSequence();
            m_ActivePattern.gameObject.SetActive(true);
        }
    }

    public void Start()
    {
        Init(m_PatternPrefabs[0]);
    }

    public void Update()
    {
        UpdateGazeUI();
        UpateSequenceRecognition();
    }

    public void UpdateGazeUI()
    {
        RaycastHit hit;
        Ray headRay = new Ray(m_HeadTransform.position, m_HeadTransform.forward);
        //if(Physics.Raycast(headRay, out hit, float.MaxValue, PatternMask))
        if (Physics.SphereCast(headRay, m_SphereCastRadius, out hit, float.MaxValue, GazeUIMask))
        {
            BaseGazeUIComponent gazeUI = hit.collider.GetComponent<BaseGazeUIComponent>();
            if (gazeUI != null)
            {
                gazeUI.GazeAt();
            }
        }
    }

    public void UpateSequenceRecognition()
    {
        if (m_ActivePattern == null || m_ActivePattern.IsComplete)
        {
            return;
        }

        m_TimeSinceLastAdvancement += Time.deltaTime;

        if (m_TimeSinceLastAdvancement > m_MaxTimeBeforeSequenceAdvancement)
        {
            m_ActivePattern.ResetSequence();
        }

        RaycastHit hit;
        Ray headRay = new Ray(m_HeadTransform.position, m_HeadTransform.forward);
        //if(Physics.Raycast(headRay, out hit, float.MaxValue, PatternMask))
        if (Physics.SphereCast(headRay, m_SphereCastRadius, out hit, float.MaxValue, PatternMask))
        {
            if (m_ActivePattern != null)
            {
                Pattern.SequenceAdvancementResult result = m_ActivePattern.AdvanceSequence(hit.collider);
                switch (result)
                {
                    case Pattern.SequenceAdvancementResult.NEW_SEQUENCE_STARTED:
                    case Pattern.SequenceAdvancementResult.SEQUENCE_ADVANCED:
                        m_TimeSinceLastAdvancement = 0;
                        break;
                    case Pattern.SequenceAdvancementResult.COMPLETE:
                        m_TimeSinceLastAdvancement = 0;
                        if (m_AutoSelectNextPattern)
                        {
                            StartCoroutine(DoWinSequence(1.0f));
                        }
                        break;
                    case Pattern.SequenceAdvancementResult.NO_MATCH:
                        m_ActivePattern.ResetSequence();
                        break;
                }
                //Debug.Log(hit.collider.name);
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
        m_ActivePattern.SetSequenceColor(Color.green);
        //m_ActivePattern.ResetSequence();
        if (m_AutoSelectNextPattern)
        {
            yield return new WaitForSeconds(0.5f);
            InitRandomPattern();
        }
    }

    public GameObject GetRandomPatternPrefab()
    {
        return m_PatternPrefabs[Random.Range(0, m_PatternPrefabs.Count)];
    }

    public void InitRandomPattern()
    {
        if (m_LastPatternPrefab != null)
        {
            m_UsedPatternPrefabs.Add(m_LastPatternPrefab);
            m_PatternPrefabs.Remove(m_LastPatternPrefab);
        }

        // reload patterns
        if (m_PatternPrefabs.Count == 0)
        {
            m_PatternPrefabs.AddRange(m_UsedPatternPrefabs);
            m_UsedPatternPrefabs.Clear();
        }

        Init(GetRandomPatternPrefab());
    }
    
}
