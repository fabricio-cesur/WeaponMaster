using UnityEngine;


public class Enemigo : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public float vidaMaxima = 30f;
    private float vidaActual;

    [Header("Movimiento y Patrulla")]
    public float velocidadPatrulla = 2f;
    public float rangoPatrulla = 3f; 
    private Vector2 puntoInicial;
    private int direccion = 1;

    [Header("Detección del Jugador")]
    public float velocidadPersecucion = 4f;
    public float distanciaDeteccion = 5f;  // Rango para empezar a seguir
    public float distanciaAbandono = 8f;   // Rango para dejar de seguir
    private Transform jugador;
    private bool persiguiendo = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        puntoInicial = transform.position;
        vidaActual = vidaMaxima;

        // Buscamos al jugador por el tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) jugador = playerObj.transform;
    }

    void Update()
    {
        if (jugador == null) return;

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        // Lógica de estados: Persecución o Patrulla
        if (distanciaAlJugador < distanciaDeteccion)
        {
            persiguiendo = true;
        }
        else if (distanciaAlJugador > distanciaAbandono)
        {
            persiguiendo = false;
        }

        if (persiguiendo)
        {
            PerseguirJugador();
        }
        else
        {
            Patrullar();
        }
    }

    void Patrullar()
    {
        // Calculamos los límites de la patrulla
        float limiteDerecha = puntoInicial.x + rangoPatrulla;
        float limiteIzquierda = puntoInicial.x - rangoPatrulla;

        if (transform.position.x >= limiteDerecha) direccion = -1;
        if (transform.position.x <= limiteIzquierda) direccion = 1;

        rb.linearVelocity = new Vector2(direccion * velocidadPatrulla, rb.linearVelocity.y);
        GirarSprite(direccion);
    }

    void PerseguirJugador()
    {
        float dirX = jugador.position.x > transform.position.x ? 1 : -1;
        rb.linearVelocity = new Vector2(dirX * velocidadPersecucion, rb.linearVelocity.y);
        GirarSprite(dirX);
    }

    void GirarSprite(float dir)
    {
        if (dir > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (dir < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    // --- SISTEMA DE VIDA (Llamado por MovimientoCaballero) ---
    public void RecibirDaño(float cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log(gameObject.name + " herido. Vida restante: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Debug.Log(gameObject.name + " ha muerto.");
        
        Destroy(gameObject);
    }

    
    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.green;
        Vector3 inicio = new Vector3(puntoInicial.x - rangoPatrulla, transform.position.y, 0);
        Vector3 fin = new Vector3(puntoInicial.x + rangoPatrulla, transform.position.y, 0);
        Gizmos.DrawLine(inicio, fin);

        // Rango de detección 
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        // Rango de abandono 
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAbandono);
    }
}