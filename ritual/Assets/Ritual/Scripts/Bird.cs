﻿using UnityEngine;
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
    protected string m_PlayedAnimationName;

	/* Rose's audio stuff */
	public AudioClip m_successDance;
	public AudioClip m_failDance;
	public AudioClip m_wedding;
	public AudioClip m_lose;

	private AudioSource m_SFXsource;

    public void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
		m_SFXsource = FindObjectOfType<AudioSource> ();
		m_SFXsource.ignoreListenerVolume = true;
    }

    public bool MoveTo(Vector3 pos)
    {
        m_Animator.SetBool("Walking", true);
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

    public void StopWalking()
    {
        m_Animator.SetBool("Walking", false);
    }

    public bool LookDir(Vector3 dir)
    {
        m_Animator.SetBool("Walking", true);
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
        StopWalking();
        m_Animator.SetFloat("X", headAnimBlend.x);
        m_Animator.SetFloat("Y", headAnimBlend.y);
    }

    public void TriggerDisappointAnimation()
    {
        TriggerAnimation("Disappoint");
		m_SFXsource.PlayOneShot(m_failDance, 1.0f);
    }

    public void TriggerApproveAnimation()
    {
        TriggerAnimation("Approve");
		m_SFXsource.PlayOneShot(m_successDance, 1.0f);
    }

    public void TriggerLoseAnimation()
    {
        TriggerAnimation("Lose");
		m_SFXsource.PlayOneShot(m_lose, 1.0f);
    }

    public void TriggerWinAnimation()
    {
        TriggerAnimation("Win");
		m_SFXsource.PlayOneShot(m_wedding, 1.0f);
    }

    public void TriggerAnimation(string name)
    {
        m_PlayedAnimationName = name;
        m_Animator.SetTrigger(name);
    }

    public bool IsPlayingAnimation()
    {
        return m_Animator.IsInTransition(0) || m_Animator.GetCurrentAnimatorStateInfo(0).IsName(m_PlayedAnimationName);
    }

    public void Dance(Pattern p, bool mirror = false)
    {
        m_Pattern = p;
        m_DanceCompleted = false;
        StartCoroutine(DoDance(mirror));
    }

    public IEnumerator DoDance(bool mirror = false)
    {
        // find center
        List<Vector2> positions = new List<Vector2>();
        Vector2 center = new Vector2(0, 4f); // m_Pattern.transform.position;//
        int sequenceLength = m_Pattern.m_PatternSequence.Length;
        for (int i = 0; i < sequenceLength; ++i)
        {
            Vector2 pos = new Vector2(m_Pattern.m_PatternSequence[i].transform.position.x, m_Pattern.m_PatternSequence[i].transform.position.y);
            if (mirror)
            {
                pos.x *= -1;
            }
            positions.Add(pos);
            //center += pos;
        }
        //center.x /= sequenceLength;
        //center.y /= sequenceLength;

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
        Vector2 desiredVec = Vector3.zero;
        for (int repeats = 0; repeats < m_Pattern.m_RequiredCompletionCount; ++repeats)
        {
            // reset data
            currentStep = 0;
            stepTimer = 0.0f;
            currVec = Vector3.zero;
            desiredVec = positions[0] - center;
            desiredVec.x /= furthest;
            desiredVec.y /= furthest;

            // use furthest point to set scaler
            while (currentStep < sequenceLength)
            {
                float t = Mathf.Clamp01(stepTimer / m_DanceStepTime);
                HeadPos(Vector2.Lerp(currVec, desiredVec, t) * 2);

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
                HeadPos(Vector2.Lerp(currVec, desiredVec, t) * 2);

                stepTimer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            HeadPos(Vector2.zero);
        }
        
        m_DanceCompleted = true;
    }
}
