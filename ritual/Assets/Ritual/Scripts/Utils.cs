using UnityEngine;
using System.Collections;

public class Utils {
    public static void SetColor(GameObject obj, Color c)
    {
        Renderer r = obj.GetComponent<Renderer>();
        if (r != null && r.material != null)
        {
            r.material.color = c;
        }
    }
}
