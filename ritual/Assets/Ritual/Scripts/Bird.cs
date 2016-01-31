using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bird : MonoBehaviour {
    public const float k_MoveToArrivalThreshold = 0.15f;
    public const float k_LookDirDotProductThreshold = 0.99f;

    public float m_Speed = 5.0f;
    public float m_MaxTurnDegreesPerSecond = 10.0f;
    public float m_DanceStepTime = 1.0f;

    protected Animator m_Animator;
    protected Pattern m_Pattern;
    protected bool m_DanceCompleted;
    public bool DanceCompleted { get { return m_DanceCompleted; } }

    public void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
    }

    public bool MoveTo(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        dir.y = 0.0f;
        float dist = dir.magnitude;
        if (dist < k_MoveToArrivalThreshold)
        {
            return true; // return true if we're within threshold
        }
        dir.Normalize();
        LookDir(dir);
        transform.position += transform.forward * m_Speed * Time.deltaTime;
        return false;
    }

    public bool LookDir(Vector3 dir)
    {
        Quaternion desiredLookRotation = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredLookRotation, m_MaxTurnDegreesPerSecond * Time.deltaTime);

        if (Vector3.Dot(dir, transform.forward) > k_LookDirDotProductThreshold)
        {
            transform.rotation = desiredLookRotation;
            return true;
        }
        return false; // if dot product is greater than .9 then we're facing close enough to the desired dir
    }

    public void HeadPos(Vector2 headAnimBlend) // X coordinate being viewer's right (1) and left (-1), Y being up (1) and down (-1) 
    {
        m_Animator.SetFloat("X", headAnimBlend.x);
        m_Animator.SetFloat("Y", headAnimBlend.y);
    }

    public void Dance(Pattern p)
    {
        m_Pattern = p;
        m_DanceCompleted = false;
        StartCoroutine(DoDance());
    }

    public IEnumerator DoDance()
    {
        // find center
        List<Vector2> positions = new List<Vector2>();
        Vector2 center = Vector2.zero;
        int sequenceLength = m_Pattern.m_PatternSequence.Length;
        for (int i = 0; i < sequenceLength; ++i)
        {
            Vector2 pos = new Vector2(m_Pattern.m_PatternSequence[i].transform.position.x, m_Pattern.m_PatternSequence[i].transform.position.y);
            positions.Add(pos);
            center += pos;
        }
        center.x /= sequenceLength;
        center.y /= sequenceLength;

        // find furthest point
        float furthest = 0.0f;
        for (int i = 0; i < sequenceLength; ++i)
        {
            float dist = (positions[i] - center).magnitude;
            if (dist > furthest)
            {
                furthest = dist;
            }
        }

        int currentStep = 0;
        float stepTimer = 0.0f;
        Vector2 currVec = Vector3.zero;
        Vector2 desiredVec = positions[0] - center;
        desiredVec.x /= furthest;
        desiredVec.y /= furthest;

        // use furthest point to set scaler
        while (currentStep < sequenceLength)
        {
            float t = Mathf.Clamp01(stepTimer/m_DanceStepTime);
            HeadPos(Vector2.Lerp(currVec, desiredVec, t));

            stepTimer += Time.deltaTime;
            if (stepTimer > m_DanceStepTime)
            {
                currentStep++;
                stepTimer = 0.0f;
                if (currentStep < sequenceLength)
                {
                    currVec = desiredVec;
                    desiredVec = positions[currentStep] - center;
                    desiredVec.x /= furthest;
                    desiredVec.y /= furthest;
                }
            }
            yield return new WaitForEndOfFrame();
        }

        stepTimer = 0.0f;
        currVec = desiredVec;
        desiredVec = Vector2.zero;
        while (stepTimer < m_DanceStepTime)
        {
            float t = Mathf.Clamp01(stepTimer / m_DanceStepTime);
            HeadPos(Vector2.Lerp(currVec, desiredVec, t));

            stepTimer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        HeadPos(Vector2.zero);
        m_DanceCompleted = true;
    }
}
