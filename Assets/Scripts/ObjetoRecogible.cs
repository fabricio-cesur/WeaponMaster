using UnityEngine;

public class ObjetoRecogible : MonoBehaviour
{
    [SerializeField] private string tipoObjeto; 
    [SerializeField] private int valor = 1;

    [Header("Persistencia")]
    [SerializeField] private string idObjeto;
    private GameManager gm;

    void Start()
    {
        gm = GameManager.gm;

        if (gm != null && gm.objetosDestruidos.Contains(idObjeto))
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out InventarioPlayer inventario))
            {
                inventario.AñadirObjeto(tipoObjeto, valor);
                gm.RegistrarObjetoDestruido(idObjeto);
                Destroy(gameObject);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
        {
            idObjeto = "";
            return;
        }

        if (string.IsNullOrEmpty(idObjeto))
        {
            idObjeto = ("recogible-" + System.Guid.NewGuid()).ToString();
            
            UnityEditor.EditorUtility.SetDirty(this); 
        }
    }
#endif
}