using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Botones de Interfaz")]
    public GameObject btnPlay;
    public GameObject btnContinue;
    public GameObject btnNewGame;
    public GameObject btnExit;

    void Start()
    {
        GameObject panelBotones = GameObject.Find("Buttons");

        if (panelBotones == null)
        {
            Debug.LogError("SISTEMA: No se ha encontrado el panel llamado 'Buttons'.");
            return;
        }

        btnPlay = panelBotones.transform.Find("BtnPlay")?.gameObject;
        btnContinue = panelBotones.transform.Find("BtnContinue")?.gameObject;
        btnNewGame = panelBotones.transform.Find("BtnNewGame")?.gameObject;
        btnExit = panelBotones.transform.Find("BtnExit")?.gameObject;

        if (btnPlay != null) btnPlay.GetComponent<Button>().onClick.AddListener(BotonJugar);
        if (btnContinue != null) btnContinue.GetComponent<Button>().onClick.AddListener(BotonContinuar);
        if (btnNewGame != null) btnNewGame.GetComponent<Button>().onClick.AddListener(BotonNuevaPartida);
        if (btnExit != null) btnExit.GetComponent<Button>().onClick.AddListener(BotonSalir);

        // Llamamos a la actualización visual al arrancar
        ActualizarBotonesInterfaz();
    }

    // Nueva función pública que el ProfileManager podrá llamar al cambiar de perfil
    public void ActualizarBotonesInterfaz()
    {
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
    if (GameManager.gm != null)
    {
        GameManager.gm.ReiniciarDatosGuardados();
        GameManager.gm.GuardarPartidaEnDisco(); 
    }
    GameSceneManager.instance.IrJuego(); 
}

    public void BotonSalir()
    {
        GameSceneManager.instance.SalirDelJuego();
    }
}