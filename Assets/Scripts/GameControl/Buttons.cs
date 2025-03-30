using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    // Método para cargar la escena "Juego" cuando se haga clic en el botón Play
    public void OnClickPlay()
    {
        Debug.Log("Play button clicked");
        // Cambiar a la escena "Juego"
        SceneManager.LoadScene("Level1");
    }

    // Método para salir del juego cuando se haga clic en el botón Exit
    public void OnClickExit()
    {
        Debug.Log("Exit button clicked");
        // Salir del juego
        Application.Quit();

        // Si estás en el editor de Unity, esto no cerrará la aplicación, pero puedes usar lo siguiente para simular el cierre:
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}