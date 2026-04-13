using System.Collections.Generic;
using UnityEngine;

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
}
