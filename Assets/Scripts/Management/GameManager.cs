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
        // Hacemos una copia exacta de la lista actual
        datos.objetosDestruidos = new System.Collections.Generic.List<string>(objetosDestruidos);

        // Convertimos los datos a texto JSON y los guardamos en el PC
        string json = JsonUtility.ToJson(datos);
        string ruta = Application.persistentDataPath + "/partida.json";
        File.WriteAllText(ruta, json);
        
        Debug.Log("Partida guardada con éxito en: " + ruta);
    }

    public bool CargarPartidaDeDisco()
    {
        string ruta = Application.persistentDataPath + "/partida.json";
        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            DatosGuardado datos = JsonUtility.FromJson<DatosGuardado>(json);

            // Volcamos los datos del archivo al GameManager
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

    public bool ExistePartidaGuardada()
    {
        return File.Exists(Application.persistentDataPath + "/partida.json");
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