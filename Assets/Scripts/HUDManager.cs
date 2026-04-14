using UnityEngine;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;


public class HUDManager : MonoBehaviour
{
    [Header("Referencias al Inventario")]
    [SerializeField] private InventarioPlayer inventario;

    [Header("Textos de la Interfaz")]
    [SerializeField] private TextMeshProUGUI textoMonedas;
    [SerializeField] private TextMeshProUGUI textoLlaves;
    [SerializeField] private TextMeshProUGUI textoPiezas;

    [Header("Sistema de Corazones")]
    [SerializeField] private PlayerController jugador;
    [SerializeField] private UnityEngine.UI.Image[] corazones;
    [SerializeField] private Sprite corazonLleno;
    [SerializeField] private Sprite corazonVacio;

    void Start()
    {
        jugador = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (inventario == null) return;

        if (textoMonedas != null) textoMonedas.text = inventario.monedas.ToString();
        if (textoLlaves != null) textoLlaves.text = "Llaves: " + inventario.llaves.ToString();
        if (textoPiezas != null) textoPiezas.text = "Piezas: " + inventario.piezasPuzle.ToString();

        if (jugador != null)
        {
            ActualizarCorazones();
        }
    }

    private void ActualizarCorazones()
    {
        // Redondeamos los valores por si acaso la vida usa decimales
        int vidaActual = Mathf.RoundToInt(jugador.saludActual);
        int vidaMaxima = Mathf.RoundToInt(jugador.saludMaxima);

        // Recorremos todos los huecos de corazones uno por uno
        for (int i = 0; i < corazones.Length; i++)
        {
            // ¿Debería verse este corazón? (Por si luego añades mejoras que suban la vida máxima a 6, 7, etc.)
            if (i < vidaMaxima)
            {
                corazones[i].enabled = true;
            }
            else
            {
                corazones[i].enabled = false;
            }

            // Si el índice del corazón es menor que tu vida, se pinta rojo. Si no, se pinta vacío.
            if (i < vidaActual)
            {
                corazones[i].sprite = corazonLleno;
            }
            else
            {
                corazones[i].sprite = corazonVacio;
            }
        }
    }
}