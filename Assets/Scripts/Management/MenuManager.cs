using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Botones de Interfaz")]
    public GameObject btnPlay;       // Tu BtnPlay original
    public GameObject btnContinue;   // Tu nuevo BtnContinue
    public GameObject btnNewGame;    // Tu nuevo BtnNewGame

    void Start()
    {
        // 1. Buscamos el panel padre por su nombre exacto
        GameObject panelBotones = GameObject.Find("Buttons");

        if (panelBotones == null)
        {
            Debug.LogError("SISTEMA: No se ha encontrado el panel llamado 'Buttons'.");
            return;
        }

        // 2. Buscamos a los hijos dentro del panel (esto encuentra objetos aunque estén apagados)
        // Usamos el ? por seguridad, por si escribes mal un nombre
        btnPlay = panelBotones.transform.Find("BtnPlay")?.gameObject;
        btnContinue = panelBotones.transform.Find("BtnContinue")?.gameObject;
        btnNewGame = panelBotones.transform.Find("BtnNewGame")?.gameObject;
        GameObject btnExit = panelBotones.transform.Find("BtnExit")?.gameObject;

        // 3. Conectamos los clics por código automáticamente (¡Magia!)
        if (btnPlay != null) btnPlay.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BotonJugar);
        if (btnContinue != null) btnContinue.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BotonContinuar);
        if (btnNewGame != null) btnNewGame.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BotonNuevaPartida);
        if (btnExit != null) btnExit.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(BotonSalir);

        // 4. Lógica de mostrar/ocultar botones según la partida guardada
        if (GameManager.gm != null && GameManager.gm.ExistePartidaGuardada())
        {
            if (btnPlay != null) btnPlay.SetActive(false);
            
            if (btnContinue != null) btnContinue.SetActive(true);
            if (btnNewGame != null) btnNewGame.SetActive(true);
        }
        else
        {
            if (btnPlay != null) btnPlay.SetActive(true);
            
            if (btnContinue != null) btnContinue.SetActive(false);
            if (btnNewGame != null) btnNewGame.SetActive(false);
        }
    }

    public void BotonJugar()
    {
        if (GameManager.gm != null) GameManager.gm.ReiniciarDatosGuardados();
        GameSceneManager.instance.IrJuego(); 
    }

    public void BotonContinuar()
    {
        if (GameManager.gm != null) GameManager.gm.CargarPartidaDeDisco();
        GameSceneManager.instance.IrJuego(); 
    }

    public void BotonNuevaPartida()
    {
        string ruta = Application.persistentDataPath + "/partida.json";
        if (System.IO.File.Exists(ruta)) 
        {
            System.IO.File.Delete(ruta);
        }
        
        if (GameManager.gm != null) GameManager.gm.ReiniciarDatosGuardados();
        GameSceneManager.instance.IrJuego(); 
    }

    public void BotonSalir()
    {
        GameSceneManager.instance.SalirDelJuego();
    }
}