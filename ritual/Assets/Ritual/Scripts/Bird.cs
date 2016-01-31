using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {
    public float m_Speed = 5.0f;
    public float m_MaxTurnDegreesPerSecond = 10.0f;

    public bool MoveTo(Vector3 pos)
    {
        Vector3 dir = pos - transform.position;
        dir.y = 0.0f;
        float dist = dir.magnitude;
        if (dist < 1.0f)
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

        return Vector3.Dot(dir, transform.forward) > 0.9f; // if dot product is greater than .9 then we're facing close enough to the desired dir
    }
}
