using UnityEngine;
using System.Collections;

public class StartGazeUI : BaseGazeUIComponent {
    public IntroManager m_IntroManager;

    public void Start()
    {
        if (m_IntroManager == null)
        {
            m_IntroManager = FindObjectOfType<IntroManager>();
        }
    }

    public override void Activate()
    {
        base.Activate();
        m_IntroManager.StartIntro();
        Destroy(gameObject);

		QuitGazeUI qgui = FindObjectOfType<QuitGazeUI>();
		TutorialGazeUI tgui = FindObjectOfType<TutorialGazeUI>();
		LogoUI lgui = FindObjectOfType<LogoUI>();

		Destroy(qgui.gameObject);
		Destroy(tgui.gameObject);
		Destroy(lgui.gameObject);
    }
}
