using UnityEngine;
using TMPro; 


public class HUDManager : MonoBehaviour
{
    [Header("Referencias al Inventario")]
    [SerializeField] private InventarioPlayer inventario;

    [Header("Textos de la Interfaz")]
    [SerializeField] private TextMeshProUGUI textoMonedas;
    [SerializeField] private TextMeshProUGUI textoLlaves;
    [SerializeField] private TextMeshProUGUI textoPiezas;

    void Update()
    {
        if (inventario == null) return;

        
        if (textoMonedas != null) 
            textoMonedas.text = "Monedas: " + inventario.monedas.ToString();

        if (textoLlaves != null) 
            textoLlaves.text = "Llaves: " + inventario.llaves.ToString();

        if (textoPiezas != null) 
            textoPiezas.text = "Piezas: " + inventario.piezasPuzle.ToString();
    }
}