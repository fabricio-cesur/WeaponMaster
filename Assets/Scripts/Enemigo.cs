using UnityEngine;

public class EnemigoIA : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    public float vidaMaxima = 30f;
    public float vidaActual;

    [Header("Ataque")]
    public float danoEnemigo = 10f;

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
    public float tiempoKnockback = 0.1f;
    private float knockbackTimer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
            damageable.RecibirDano(danoEnemigo, transform.position, 20f);
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