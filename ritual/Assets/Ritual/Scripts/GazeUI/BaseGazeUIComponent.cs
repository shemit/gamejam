using UnityEngine;
using System.Collections;

public class BaseGazeUIComponent : MonoBehaviour {
    public float m_ActivationTime = 3.0f;
    public Color m_ActivationColor = Color.green;

    protected float m_GazeTimer = 0.0f;

    protected bool m_WasGazedAt = false;
    public void GazeAt()
    {
        m_WasGazedAt = true;
    }

    public void LateUpdate()
    {
        if (m_WasGazedAt)
        {
            m_GazeTimer += Time.deltaTime;
            //Debug.Log("Gazing: " + m_GazeTimer);
            if (m_GazeTimer >= m_ActivationTime)
            {
                Activate();
                m_GazeTimer = 0.0f;
            }
        }
        else
        {
            m_GazeTimer = 0.0f;
        }

        UpdateFeedback();
        m_WasGazedAt = false;
    }

    public virtual void UpdateFeedback()
    {
        Utils.SetColor(gameObject, Color.Lerp(Color.white, m_ActivationColor, m_GazeTimer/m_ActivationTime));
    }

    public virtual void Activate()
    {

    }
}
