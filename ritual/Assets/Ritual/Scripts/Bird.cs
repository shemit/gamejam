using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {
    public const float k_MoveToArrivalThreshold = 0.15f;
    public const float k_LookDirDotProductThreshold = 0.99f;

    public float m_Speed = 5.0f;
    public float m_MaxTurnDegreesPerSecond = 10.0f;

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
        Animator animator = GetComponentInChildren<Animator>();

        animator.SetFloat("X", headAnimBlend.x);
        animator.SetFloat("Y", headAnimBlend.y);
    }
}
