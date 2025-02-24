using UnityEngine;

public class ScriptMenu : MonoBehaviour
{
    public Canvas canvasWin;
    public Canvas canvasLose;
    
    public void ShowWin()
    {
        Debug.Log("Win");
        canvasWin.enabled = true;
    }
    
    public void ShowLose()
    {
        canvasLose.enabled = true;
    }
}
