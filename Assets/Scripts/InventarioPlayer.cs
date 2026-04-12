using UnityEngine;

public class InventarioPlayer : MonoBehaviour
{
    [Header("Contadores de Objetos")]
    public int monedas;
    public int llaves;
    public int piezasPuzle;

    public void AñadirObjeto(string tipo, int cantidad)
    {
        // ToLower para evitar errores
        switch (tipo.ToLower())
        {
            case "moneda":
                monedas += cantidad;
                break;
            case "llave":
                llaves += cantidad;
                break;
            case "pieza":
                piezasPuzle += cantidad;
                break;
            default:
                Debug.LogWarning($"INVENTARIO: El tipo '{tipo}' no esta definido.");
                break;
        }

        Debug.Log($"INVENTARIO: +{cantidad} {tipo}. Total: {ObtenerCantidad(tipo)}");
    }

    public int ObtenerCantidad(string tipo)
    {
        return tipo.ToLower() switch
        {
            "moneda" => monedas,
            "llave" => llaves,
            "pieza" => piezasPuzle,
            _ => 0
        };
    }
}