using UnityEngine;

public class InventarioPlayer : MonoBehaviour
{
    [Header("Contadores de Objetos")]
    public int monedas 
    { 
        get { return GameManager.gm != null ? GameManager.gm.monedas : 0; } 
        set { if (GameManager.gm != null) GameManager.gm.monedas = value; } 
    }

    public int llaves 
    { 
        get { return GameManager.gm != null ? GameManager.gm.llaves : 0; } 
        set { if (GameManager.gm != null) GameManager.gm.llaves = value; } 
    }

    public int piezasPuzle 
    { 
        get { return GameManager.gm != null ? GameManager.gm.piezasPuzle : 0; } 
        set { if (GameManager.gm != null) GameManager.gm.piezasPuzle = value; } 
    }

    [Header("Persistencia")]
    private GameManager gm;

    void Start()
    {
        gm = GameManager.gm;
    }

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

    public void GuardarInventario()
    {
    }
}