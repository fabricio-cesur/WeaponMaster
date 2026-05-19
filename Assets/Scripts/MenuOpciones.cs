using System.Collections.Generic;
using UnityEngine;
using TMPro; // Usa using UnityEngine.UI; si tu Dropdown no es de TextMeshPro

public class MenuOpciones : MonoBehaviour
{
    public TMP_Dropdown dropdownResolucion; // Arrastra tu Dropdown aquí en el Inspector
    private Resolution[] resolucionesSoportadas;

    void Start()
    {
        // 1. Recogemos las resoluciones que permite la pantalla del jugador
        resolucionesSoportadas = Screen.resolutions;

        // Limpiamos las opciones por defecto del Dropdown
        dropdownResolucion.ClearOptions();

        // 2. Creamos una lista de textos para mostrar en el menú desplegable
        List<string> opciones = new List<string>();
        int indiceResolucionActual = 0;

        for (int i = 0; i < resolucionesSoportadas.Length; i++)
        {
            string opcion = resolucionesSoportadas[i].width + " x " + resolucionesSoportadas[i].height;
            opciones.Add(opcion);

            // Detectamos cuál es la resolución que tiene el monitor ahora mismo para dejarla seleccionada
            if (resolucionesSoportadas[i].width == Screen.currentResolution.width &&
                resolucionesSoportadas[i].height == Screen.currentResolution.height)
            {
                indiceResolucionActual = i;
            }
        }

        // 3. Metemos la lista en el Dropdown y marcamos la actual
        dropdownResolucion.AddOptions(opciones);
        dropdownResolucion.value = indiceResolucionActual;
        dropdownResolucion.RefreshShownValue();
    }

    // 4. Este método lo llamará el Dropdown cuando el jugador cambie la opción
    public void CambiarResolucion(int indiceResolucion)
    {
        Resolution resolucion = resolucionesSoportadas[indiceResolucion];
        
        // Cambia la resolución manteniendo si el juego está en pantalla completa o ventana
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }
}