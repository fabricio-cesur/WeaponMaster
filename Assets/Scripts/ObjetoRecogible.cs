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
    [ContextMenu("Forzar Nuevo ID")]
    public void ForzarNuevoID()
    {
        // Registramos el cambio para que funcione el Ctrl+Z por si te equivocas
        UnityEditor.Undo.RecordObject(this, "Generar ID Único"); 
        
        idObjeto = ("recogible-" + System.Guid.NewGuid()).ToString();
        
        // Le avisamos a la escena de que hay cambios sin guardar
        UnityEditor.EditorUtility.SetDirty(this);
    }

    // 2. La magia para hacer 50 objetos a la vez
    // Esto crea un botón al hacer clic derecho sobre los objetos en la ventana de Jerarquía
    [UnityEditor.MenuItem("GameObject/Generar Nuevos IDs para Recogibles", false, 0)]
    private static void GenerarIDsMultiples()
    {
        // Recorremos todos los objetos que tengas seleccionados con el ratón
        foreach (GameObject obj in UnityEditor.Selection.gameObjects)
        {
            // OJO: Cambia 'Enemigo' por el nombre exacto de tu script (ej. Moneda, Cofre...)
            ObjetoRecogible script = obj.GetComponent<ObjetoRecogible>(); 
            
            if (script != null)
            {
                script.ForzarNuevoID();
            }
        }
    }
#endif
}