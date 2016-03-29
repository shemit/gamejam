using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
	GameObject currTutorialScreen;

	public PatternRecognizer m_PatternRecognizer;
	public HealthIndicator m_HealthIndicator;
	public int m_MaxTries = 3;
	protected int m_NumTriesLeft = 0;
	public GameObject tutorialText;
	public GameObject textBg;
	public int phase_idx = 0;
	protected Canvas _canvas;
	public GameObject screen1;
	public GameObject screen2;

	public enum TutorialPhase {
		NOT_STARTED,
		WELCOME,
		LOCATOR,
		FINISHED
	}

	public List<TutorialPhase> phases;

	protected TutorialPhase m_CurrentPhase = TutorialPhase.NOT_STARTED;

	public void Start() {
		phases = new List<TutorialPhase>();
		phases.Add(TutorialPhase.NOT_STARTED);
		phases.Add(TutorialPhase.WELCOME);
		phases.Add(TutorialPhase.LOCATOR);
		phases.Add(TutorialPhase.FINISHED);

		if (m_PatternRecognizer == null)
		{
			m_PatternRecognizer = FindObjectOfType<PatternRecognizer>();
		}

		if (m_HealthIndicator == null)
		{
			m_HealthIndicator = FindObjectOfType<HealthIndicator>();
		}
		m_HealthIndicator.Init(m_MaxTries);

	}
	bool enableTutorialUI = false;
	public void StartTutorial() {
		enableTutorialUI = true;
		m_NumTriesLeft = m_MaxTries;
		m_PatternRecognizer.PauseSequenceRecognition();
		m_CurrentPhase = TutorialPhase.WELCOME;
	}

	public void nextPhase() {
		phase_idx++;
		m_CurrentPhase = phases[phase_idx];
		startEnter = false;
	}

	protected bool isTouching = false;
	public void Update() {
		
		if ((Input.touchCount > 0 || Input.GetMouseButtonDown(0)) && enableTutorialUI) {
			isTouching = true;
			Debug.Log("Got a touch time");

		} else {
			if (isTouching) {
				isTouching = false;
				nextPhase();
			}
		}

		switch (m_CurrentPhase)
		{
			case TutorialPhase.WELCOME:
				HandleWelcomePhase();
				break;
			case TutorialPhase.LOCATOR:
				HandleLocatorPhase();
				break;
			case TutorialPhase.FINISHED:
				HandleFinishedPhase();
				break;
		}
	}

	protected float startEnterTime;
	protected bool startEnter = false;

	public void HandleWelcomePhase() {
		// TODO: Break this logic into timer.
		if (!startEnter) {
			startEnter = true;
			startEnterTime = Time.time;
			screen1.transform.localPosition = Vector3.zero;

		} 
		/*
		else {
			if (Time.time - startEnterTime > 5) {
				startEnter = false;
				m_CurrentPhase = TutorialPhase.LOCATOR;
			}
		}
		*/
		//Debug.Log("Inside welcome");
	}

	public void HandleLocatorPhase() {
		//Debug.Log("In locator phase");
		if (!startEnter) {
			startEnter = true;
			startEnterTime = Time.time;
			screen1.transform.localPosition = new Vector3(0f, -400f, 0f);
			screen2.transform.localPosition = Vector3.zero;

		}
	}

	public void HandleFinishedPhase() {
		Application.LoadLevel (0);
	}

	public void Activate()
	{
		currTutorialScreen = GameObject.CreatePrimitive(PrimitiveType.Plane);
		currTutorialScreen.transform.position = Vector3.zero;
	}
}

