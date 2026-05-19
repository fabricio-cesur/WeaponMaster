using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    [Header("Datos del Jugador")]
    public Vector3 posicionJugador;
    public float saludJugador;
    public bool tieneDatos = false;

    [Header("Inventario jugador")]
    public int monedas = 0;
    public int llaves = 0;
    public int piezasPuzle = 0;

    [Header("Mundo")]
    public List<string> objetosDestruidos = new List<string>();

    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegistrarObjetoDestruido(string idObjeto)
    {
        if (!objetosDestruidos.Contains(idObjeto))
        {
            objetosDestruidos.Add(idObjeto);
        }
    }

    public void ReiniciarDatosGuardados()
    {
        tieneDatos = false;
        posicionJugador = Vector3.zero;
        saludJugador = 5f;

        monedas = 0;
        llaves = 0;
        piezasPuzle = 0;

        if (objetosDestruidos != null)
        {
            objetosDestruidos.Clear();
        }
    }

    // Funcionamiento para guardado permanente en disco:
    public void GuardarPartidaEnDisco()
    {
        DatosGuardado datos = new DatosGuardado();
        datos.posX = posicionJugador.x;
        datos.posY = posicionJugador.y;
        datos.posZ = posicionJugador.z;
        datos.salud = saludJugador;
        datos.monedas = monedas;
        datos.llaves = llaves;
        datos.piezas = piezasPuzle;
        datos.objetosDestruidos = new System.Collections.Generic.List<string>(objetosDestruidos);

        string json = JsonUtility.ToJson(datos);
        
        // ¡AQUÍ ESTABA EL ERROR! Ahora usamos la ruta del perfil actual
        string ruta = ObtenerRutaGuardadoActual();
        
        File.WriteAllText(ruta, json);
        
        Debug.Log("Partida guardada con éxito en: " + ruta);
    }

    public bool CargarPartidaDeDisco()
    {
        // ¡AQUÍ TAMBIÉN! Usamos la ruta del perfil
        string ruta = ObtenerRutaGuardadoActual();
        
        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

            posicionJugador = new Vector3(datos.posX, datos.posY, datos.posZ);
            saludJugador = datos.salud;
            monedas = datos.monedas;
            llaves = datos.llaves;
            piezasPuzle = datos.piezas;
            objetosDestruidos = new System.Collections.Generic.List<string>(datos.objetosDestruidos);
            tieneDatos = true;
            return true;
        }
        return false;
    }

    // Cambiado de "Invitado" a "Jugador"
    public string perfilActivo = "Jugador"; 
    
    public void EstablecerPerfil(string nombrePerfil)
    {
        perfilActivo = nombrePerfil;
    }

    public string ObtenerRutaGuardadoActual()
    {
        return Application.persistentDataPath + "/perfil_" + perfilActivo + ".json";
    }

    public bool ExistePartidaGuardada()
    {
        string ruta = ObtenerRutaGuardadoActual();
        
        if (!System.IO.File.Exists(ruta)) return false;
        
        string contenido = System.IO.File.ReadAllText(ruta);
        
        if (contenido.Length <= 5) return false; 
        
        return true; 
    }
}

[System.Serializable]
public class DatosGuardado
{
    public float posX, posY, posZ;
    public float salud;
    public int monedas, llaves, piezas;
    public System.Collections.Generic.List<string> objetosDestruidos;
}