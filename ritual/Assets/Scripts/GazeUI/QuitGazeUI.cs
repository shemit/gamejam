using UnityEngine;
using System.Collections;

public class QuitGazeUI : BaseGazeUIComponent {

    public override void Activate()
    {
        base.Activate();
        Debug.Log("quiting");
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
