using UnityEngine;

public class EnemigoIA : MonoBehaviour, IDamageable
{

    [Header("Persistencia")]
    private GameManager gm;
    public string idObjeto;

    [Header("Salud")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Ataque")]
    public float danoEnemigo = 1f;

    [Header("Patrulla")]
    public float velocidadPatrulla = 3f;
    public float rangoPatrulla = 5f; 
    private float xMinima;
    private float xMaxima;
    private int direccionActual = 1;
    private Vector3 escalaOriginal;

    [Header("Persecución")]
    public float velocidadPersecucion = 5f;
    public float distanciaDeteccion = 6f;  
    public float distanciaAbandono = 10f;   
    private Transform transformJugador;
    private bool estaPersiguiendo = false;

    [Header("Knockback")]
    public float fuerzaEmpujeAtaque = 20f;
    public float tiempoKnockback = 0.1f;
    private float knockbackTimer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gm = GameManager.gm;

        if (gm != null && gm.objetosDestruidos.Contains(idObjeto))
        {
            Destroy(gameObject);
            return;
        }
        escalaOriginal = transform.localScale;
        xMinima = transform.position.x - rangoPatrulla;
        xMaxima = transform.position.x + rangoPatrulla;
        vidaActual = vidaMaxima;
        BuscarJugador();
    }

    void BuscarJugador()
    {
        GameObject jugadorObjetivo = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObjetivo != null) transformJugador = jugadorObjetivo.transform;
    }

    void FixedUpdate() 
    {
        if (rb == null) return;

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime; 

            return; 
        }

        if (transformJugador == null) BuscarJugador();

        if (transformJugador != null)
        {
            float distancia = Vector2.Distance(transform.position, transformJugador.position);
            if (distancia < distanciaDeteccion) estaPersiguiendo = true;
            else if (distancia > distanciaAbandono) estaPersiguiendo = false;
        }

        if (estaPersiguiendo && transformJugador != null) Perseguir();
        else Patrullar();
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
            idObjeto = ("enemigo-" + System.Guid.NewGuid()).ToString();
            
            UnityEditor.EditorUtility.SetDirty(this); 
        }
    }
    [ContextMenu("Forzar Nuevo ID")]
    public void ForzarNuevoID()
    {
        // Registramos el cambio para que funcione el Ctrl+Z por si te equivocas
        UnityEditor.Undo.RecordObject(this, "Generar ID Único"); 
        
        idObjeto = ("enemigo-" + System.Guid.NewGuid()).ToString();
        
        // Le avisamos a la escena de que hay cambios sin guardar
        UnityEditor.EditorUtility.SetDirty(this);
    }

    // 2. La magia para hacer 50 objetos a la vez
    // Esto crea un botón al hacer clic derecho sobre los objetos en la ventana de Jerarquía
    [UnityEditor.MenuItem("GameObject/Persistencia/Generar IDs para Enemigos", false, 0)]
    private static void GenerarIDsMultiples()
    {
        // Recorremos todos los objetos que tengas seleccionados con el ratón
        foreach (GameObject obj in UnityEditor.Selection.gameObjects)
        {
            // OJO: Cambia 'Enemigo' por el nombre exacto de tu script (ej. Moneda, Cofre...)
            EnemigoIA script = obj.GetComponent<EnemigoIA>(); 
            
            if (script != null)
            {
                script.ForzarNuevoID();
            }
        }
    }
#endif

    void Patrullar()
    {
        if (transform.position.x >= xMaxima) direccionActual = -1;
        if (transform.position.x <= xMinima) direccionActual = 1;
        rb.linearVelocity = new Vector2(direccionActual * velocidadPatrulla, rb.linearVelocity.y);
        GirarSprite(direccionActual);
    }

    void Perseguir()
    {
        float direccionHaciaJugador = transformJugador.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(direccionHaciaJugador * velocidadPersecucion, rb.linearVelocity.y);
        GirarSprite(direccionHaciaJugador);
    }

    void GirarSprite(float dirX)
    {
        transform.localScale = new Vector3(escalaOriginal.x * dirX, escalaOriginal.y, escalaOriginal.z);
    }

    public void ModificarVida(float cantidad)
    {
        vidaActual += cantidad;
        if (vidaActual <= 0) Morir();
    }

    void Morir()
    {
        if (gm != null)
        {
            gm.RegistrarObjetoDestruido(idObjeto);
        }
        if (transform.parent != null) Destroy(transform.parent.gameObject);
        else Destroy(gameObject);
    }

    public void RecibirDano(float cantidadDano, Vector2 posicionAtacante, float fuerzaEmpuje = 10f)
    {
        vidaActual -= cantidadDano;
        AplicarKnockback(posicionAtacante, fuerzaEmpuje);
        Debug.Log($"ENEMIGO: Vida restante de {vidaActual} puntos");
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void AplicarKnockback(Vector2 posicionAtacante, float fuerzaEmpuje)
    {
        // Si no hay empuje no hace falta aplicar el Knockback
        if (fuerzaEmpuje <= 0) return;

        knockbackTimer = tiempoKnockback;

        float direccionAtaqueX = transform.position.x > posicionAtacante.x ? 1 : -1;
        Vector2 direccionEmpuje = new Vector2(direccionAtaqueX, 0.5f).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = direccionEmpuje * fuerzaEmpuje;
    }

    //Hacer daño a player
    private void OnCollisionEnter2D(Collision2D otro)
    {
        GameObject otroObjeto = otro.gameObject;
        string tag = otroObjeto.tag;
        
        if (otroObjeto.GetComponent<IDamageable>() != null)
        {
            IDamageable damageable = otroObjeto.GetComponent<IDamageable>();
            Debug.Log($"ENEMIGO: Daño a {otro.gameObject.name} de {danoEnemigo} puntos.");
            damageable.RecibirDano(danoEnemigo, transform.position, fuerzaEmpujeAtaque);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        float visualMin = Application.isPlaying ? xMinima : transform.position.x - rangoPatrulla;
        float visualMax = Application.isPlaying ? xMaxima : transform.position.x + rangoPatrulla;
        Gizmos.DrawLine(new Vector2(visualMin, transform.position.y), new Vector2(visualMax, transform.position.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
    }
}