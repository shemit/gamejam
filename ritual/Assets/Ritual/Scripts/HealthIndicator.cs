using UnityEngine;
using System.Collections.Generic;

public class HealthIndicator : MonoBehaviour {
    public GameObject m_HealthIndicatorPrefab;
    public Transform m_HealthIndicatorCenter;
    public float m_DistanceBetweenIndicators = 0.2f;
    protected List<GameObject> m_HealthIndicators = new List<GameObject>();

    public void Init(int startingHP)
    {
        Vector3 offset = Vector3.zero;
        float offsetAmount = (startingHP-1)/2;
        offset -= Vector3.right * m_DistanceBetweenIndicators * offsetAmount;

        for (int i = 0; i < startingHP; ++i)
        {
            GameObject obj = Instantiate(m_HealthIndicatorPrefab, m_HealthIndicatorCenter.position, m_HealthIndicatorCenter.rotation) as GameObject;
            obj.transform.SetParent(m_HealthIndicatorCenter);
            obj.transform.localPosition = offset;
            m_HealthIndicators.Add(obj);

            offset += Vector3.right * m_DistanceBetweenIndicators;
        }
    }

    public void DeductHP()
    {
        if (m_HealthIndicators.Count > 0)
        {
            GameObject obj = m_HealthIndicators[0];
            m_HealthIndicators.Remove(obj);
            Destroy(obj);
        }
    }
}
