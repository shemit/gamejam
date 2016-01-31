using UnityEngine;
using System.Collections;

public class QuitGazeUI : BaseGazeUIComponent {

    public override void Activate()
    {
        base.Activate();
        Debug.Log("quiting");
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
#endif
        Application.Quit();
    }
}
