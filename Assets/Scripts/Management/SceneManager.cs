using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class SceneManager : MonoBehaviour
{
    public string escenaJuego; //nombre de la escena del Juego
    public string escenaMenu; //nombre de la escena del menu

    public void irMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaMenu);
    }
    public void irJuego()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaJuego);
    }

    public void salirDelJuego()
    {
    //si está en el editor de unity parar el juego
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
    //si está abierto cómo aplicación, cerrar el juego
            Application.Quit();
    #endif
    }
}
