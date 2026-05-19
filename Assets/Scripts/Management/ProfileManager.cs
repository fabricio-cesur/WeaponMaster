using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    [Header("Arrastra aquí el BtnPerfil desde Assets/Prefabs")]
    [SerializeField] private GameObject prefabPerfilSlot; 

    // Variables internas que se asignan solas
    private Button btnPerfilActual;
    private TextMeshProUGUI txtPerfilActual;
    private GameObject panelListaPerfiles;
    private Transform contenedorLista;   
    
    private GameObject panelCrearPerfil; 
    private TMP_InputField inputNombrePerfil;
    
    private MenuManager menuManager;
    private string rutaGuardado;

    void Start()
    {
        menuManager = GetComponent<MenuManager>();
        rutaGuardado = Application.persistentDataPath;

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("SISTEMA: No se encontró el Canvas.");
            return;
        }

        Transform menu = canvas.transform.Find("Menu");
        if (menu != null)
        {
            Transform profiles = menu.Find("Profiles");
            if (profiles != null)
            {
                Transform btnCurrentObj = profiles.Find("BtnCurrentProfile");
                if (btnCurrentObj != null)
                {
                    btnPerfilActual = btnCurrentObj.GetComponent<Button>();
                    btnPerfilActual.onClick.AddListener(AlternarPanelPerfiles);
                    
                    Transform txtCurrentObj = btnCurrentObj.Find("TxtCurrentProfile");
                    if (txtCurrentObj != null)
                        txtPerfilActual = txtCurrentObj.GetComponent<TextMeshProUGUI>();
                }

                Transform profileListObj = profiles.Find("ProfileList");
                if (profileListObj != null)
                {
                    panelListaPerfiles = profileListObj.gameObject;
                    
                    // NUEVO: Buscamos el Content dentro del Scroll View
                    Transform contentObj = profileListObj.Find("Scroll View/Viewport/Content");
                    if (contentObj != null)
                    {
                        contenedorLista = contentObj;
                    }

                    Transform btnCreateObj = profileListObj.Find("BtnCreateProfile");
                    if (btnCreateObj != null)
                    {
                        btnCreateObj.GetComponent<Button>().onClick.RemoveAllListeners();
                        btnCreateObj.GetComponent<Button>().onClick.AddListener(AbrirVentanaCreacion);
                    }
                }
            }

            Transform panelCreateObj = menu.Find("PanelCreateProfile");
            if (panelCreateObj != null)
            {
                panelCrearPerfil = panelCreateObj.gameObject;
                
                Transform inputObj = panelCreateObj.Find("InputName");
                if (inputObj != null) 
                {
                    inputNombrePerfil = inputObj.GetComponent<TMP_InputField>();
                    
                    // Esta línea mágica detecta el "Enter" y llama a tu función
                    inputNombrePerfil.onSubmit.AddListener((textoEscrito) => ConfirmarCrearPerfil());
                }

                Transform btnConfirmObj = panelCreateObj.Find("BtnConfirm");
                if (btnConfirmObj != null) btnConfirmObj.GetComponent<Button>().onClick.AddListener(ConfirmarCrearPerfil);
            }
        }

        // Comprobación de seguridad para el Prefab
        if (prefabPerfilSlot == null)
        {
            Debug.LogError("SISTEMA: Falta arrastrar el prefab 'BtnPerfil' en el Inspector del ProfileManager.");
        }

        // Ocultamos los paneles al arrancar
        if (panelListaPerfiles != null) panelListaPerfiles.SetActive(false);
        if (panelCrearPerfil != null) panelCrearPerfil.SetActive(false);

        CargarPerfilesDesdeDisco();
    }

    public void AlternarPanelPerfiles()
    {
        if (panelListaPerfiles != null)
        {
            
            panelListaPerfiles.SetActive(!panelListaPerfiles.activeSelf);
        }
        else
        {
            Debug.LogError("SISTEMA: El panelListaPerfiles es nulo, no se puede alternar.");
        }
    }

    public void CargarPerfilesDesdeDisco()
    {
        // Limpiamos la lista visual, exceptuando el botón de crear
        foreach (Transform child in contenedorLista)
        {
            Destroy(child.gameObject);
        }

        string[] archivos = Directory.GetFiles(rutaGuardado, "perfil_*.json");

        // Si no hay absolutamente ningún archivo en el disco, CREAMOS uno por defecto
        if (archivos.Length == 0)
        {
            string rutaPorDefecto = rutaGuardado + "/perfil_Jugador.json";
            File.WriteAllText(rutaPorDefecto, "{}");
            archivos = new string[] { rutaPorDefecto }; // Lo añadimos a la lista de lectura
        }

        // Instanciamos los botones
        foreach (string ruta in archivos)
        {
            string nombreArchivo = Path.GetFileNameWithoutExtension(ruta);
            string nombrePerfil = nombreArchivo.Replace("perfil_", "");
            CrearBotonVisualPerfil(nombrePerfil);
        }

        // --- LÓGICA DE SELECCIÓN INTELIGENTE ---
        string perfilARecuperar = "";
        
        // Preguntamos al GameManager si ya había un perfil activo (por si venimos de jugar un nivel)
        if (GameManager.gm != null && !string.IsNullOrEmpty(GameManager.gm.perfilActivo))
        {
            perfilARecuperar = GameManager.gm.perfilActivo;
        }

        // Comprobamos si ese perfil que queremos recuperar existe físicamente
        bool perfilExisteEnDisco = File.Exists(rutaGuardado + "/perfil_" + perfilARecuperar + ".json");

        if (perfilExisteEnDisco)
        {
            // Si existe y ya estaba activo, lo volvemos a seleccionar sin cambiar nada
            SeleccionarPerfil(perfilARecuperar);
        }
        else
        {
            // Si no (es la primera vez que abrimos el juego), seleccionamos el primero de la lista
            string primerPerfil = Path.GetFileNameWithoutExtension(archivos[0]).Replace("perfil_", "");
            SeleccionarPerfil(primerPerfil);
        }
    }

    private void CrearBotonVisualPerfil(string nombrePerfil)
    {
        GameObject nuevoSlot = Instantiate(prefabPerfilSlot, contenedorLista);
        nuevoSlot.name = "Slot_" + nombrePerfil;

        Transform txtProfileObj = nuevoSlot.transform.Find("TxtProfile");
        if (txtProfileObj != null)
        {
            txtProfileObj.GetComponent<TextMeshProUGUI>().text = nombrePerfil;
        }

        Button btnPrincipal = nuevoSlot.GetComponent<Button>();
        btnPrincipal.onClick.AddListener(() => {
            SeleccionarPerfil(nombrePerfil);
            panelListaPerfiles.SetActive(false); 
        });

        Transform trashcanObj = nuevoSlot.transform.Find("Trashcan");
        if (trashcanObj != null)
        {
            trashcanObj.GetComponent<Button>().onClick.AddListener(() => BorrarPerfil(nombrePerfil, nuevoSlot));
        }

        Transform configObj = nuevoSlot.transform.Find("Configuration");
        if (configObj != null)
        {
            configObj.GetComponent<Button>().onClick.AddListener(() => {
                Debug.Log("SISTEMA: Abrir opciones para el perfil -> " + nombrePerfil);
            });
        }
    }

    public void SeleccionarPerfil(string nombre)
    {
        if (txtPerfilActual != null)
        {
            txtPerfilActual.text = nombre;
        }

        if (GameManager.gm != null)
        {
            GameManager.gm.EstablecerPerfil(nombre); 
        }

        if (menuManager != null)
        {
            menuManager.ActualizarBotonesInterfaz();
        }
    }

    public void BorrarPerfil(string nombre, GameObject slotVisual)
    {
        string ruta = rutaGuardado + "/perfil_" + nombre + ".json";
        
        if (File.Exists(ruta))
        {
            File.Delete(ruta);
            Debug.Log("SISTEMA: Perfil borrado -> " + nombre);
        }

        Destroy(slotVisual);

        // Si se borra el perfil que está seleccionado en ese momento, recargamos para seleccionar otro válido
        if (txtPerfilActual.text == nombre)
        {
            CargarPerfilesDesdeDisco();
        }
    }

    public void AbrirVentanaCreacion()
    {
        if (panelCrearPerfil != null)
        {
            panelCrearPerfil.SetActive(true);
            panelListaPerfiles.SetActive(false); 
            if (inputNombrePerfil != null) inputNombrePerfil.text = "";
        }
    }

    public void ConfirmarCrearPerfil()
    {
        if (inputNombrePerfil == null) return;

        string nuevoNombre = inputNombrePerfil.text.Trim();

        if (!string.IsNullOrEmpty(nuevoNombre))
        {
            string ruta = rutaGuardado + "/perfil_" + nuevoNombre + ".json";
            
            if (!File.Exists(ruta))
            {
                File.WriteAllText(ruta, "{}"); 
            }

            if (panelCrearPerfil != null) panelCrearPerfil.SetActive(false);
            
            CargarPerfilesDesdeDisco();
            SeleccionarPerfil(nuevoNombre);
        }
    }
}