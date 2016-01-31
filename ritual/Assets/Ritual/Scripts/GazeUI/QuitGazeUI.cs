using UnityEngine;
using System.Collections;

public class QuitGazeUI : BaseGazeUIComponent {

    public override void Activate()
    {
        base.Activate();
#if UNITY_EDITOR
        Debug.Log("quiting");
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif

        Application.Quit();
    }
}
