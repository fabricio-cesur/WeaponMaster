using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class GameSceneManager : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    [SerializeField] private string escenaJuego; //nombre de la escena del Juego
    [SerializeField] private string escenaMenu; //nombre de la escena del menu
    
    //Singleton, asegurarse que sólo haya 1 GameSceneManager por escena.
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestoyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IrMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaMenu);
    }
    public void IrJuego()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(escenaJuego);
    }

    public void SalirDelJuego()
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
