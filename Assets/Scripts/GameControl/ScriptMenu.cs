using UnityEngine;

public class ScriptMenu : MonoBehaviour
{
    [SerializeField] private Canvas canvasWin;
    [SerializeField] private Canvas canvasLose;
    
    private void Start()
    {
        canvasWin.enabled = false;
        canvasLose.enabled = false;
    }
    
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
