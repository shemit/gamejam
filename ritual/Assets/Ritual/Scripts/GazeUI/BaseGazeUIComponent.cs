using UnityEngine;
using System.Collections;

public class BaseGazeUIComponent : MonoBehaviour {
    public float m_ActivationTime = 3.0f;
    public Color m_ActivationColor = Color.green;
	public Material swapMaterial;
	public Material mainMaterial;

    protected float m_GazeTimer = 0.0f;

    protected bool m_WasGazedAt = false;
    public void GazeAt()
    {
        m_WasGazedAt = true;
    }

    public void LateUpdate()
    {
		if (m_WasGazedAt && (Input.touchCount > 0 || Input.GetMouseButton(0)))
        {
            //m_GazeTimer += Time.deltaTime;
            //Debug.Log("Gazing: " + m_GazeTimer);
            //if (m_GazeTimer >= m_ActivationTime)
            //{
                Activate();
                m_GazeTimer = 0.0f;
            //}
		} else if (m_WasGazedAt) {
			SwapGazeColor();
		}
        else
        {
            m_GazeTimer = 0.0f;
			SwapGazeOriginalColor();
        }

        //UpdateFeedback();
        m_WasGazedAt = false;
    }

    public virtual void SwapGazeColor()
    {
        //Utils.SetColor(gameObject, Color.Lerp(Color.white, m_ActivationColor, m_GazeTimer/m_ActivationTime));
		//Renderer mr = gameObject.GetComponent<Renderer>();
		Renderer[] mr = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in mr) {
			if (mr != null) {
				foreach (Material mat in rend.materials) {
					mat.SetColor("_Color", new Color(1, 1, 1, 1));
				}
			}
		}
    }

	public virtual void SwapGazeOriginalColor()
	{
		//Utils.SetColor(gameObject, Color.Lerp(Color.white, m_ActivationColor, m_GazeTimer/m_ActivationTime));
		//Renderer mr = gameObject.GetComponent<Renderer>();
		Renderer[] mr = gameObject.GetComponentsInChildren<Renderer>();
		foreach (Renderer rend in mr) {
			if (mr != null) {
				foreach (Material mat in rend.materials) {
					mat.SetColor("_Color", new Color(221.0f/256.0f, 213.0f/256.0f, 151.0f/256.0f, 1.0f));
				}
			}
		}
	}

    public virtual void Activate()
    {

    }
}
