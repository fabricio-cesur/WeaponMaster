using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    [SerializeField] private string tipoObjeto; 
    [SerializeField] private int valor = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out InventarioPlayer inventario))
            {
                inventario.AñadirObjeto(tipoObjeto, valor);
                Destroy(gameObject);
        }
    }
}
}