using UnityEngine;
using System.Collections;

public class TutorialGazeUI : BaseGazeUIComponent {
	public TutorialManager m_TutorialManager;

	public void Start()
	{
		if (m_TutorialManager == null)
		{
			m_TutorialManager = FindObjectOfType<TutorialManager>();
		}

	}

	public override void Activate()
	{
		base.Activate();
		m_TutorialManager.StartTutorial();
		Destroy(gameObject);

		QuitGazeUI qgui = FindObjectOfType<QuitGazeUI>();
		StartGazeUI sgui = FindObjectOfType<StartGazeUI>();
		LogoUI lgui = FindObjectOfType<LogoUI>();

		Destroy(qgui.gameObject);
		Destroy(sgui.gameObject);
		Destroy(lgui.gameObject);
	}
}
