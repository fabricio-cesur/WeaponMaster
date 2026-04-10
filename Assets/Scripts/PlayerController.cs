using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    public float saludMaxima = 100f;
    public float saludActual;
    public float tiempoInvencibilidad = 1f;
    private float timerInvencibilidad;

    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;
    private Rigidbody2D rb;
    private float movimientoX;

    [Header("Coyote & Buffer")]
    public float tiempoCoyoteMax = 0.15f; 
    private float coyoteTimer;
    public float tiempoBufferMax = 0.1f;
    private float bufferTimer;

    [Header("Ataque")]
    [SerializeField] private GameObject prefabAtaque;
    [SerializeField] private float tiempoVidaAtaque = 0.1f;
    public float tiempoEntreAtaques = 0.3f;
    private float cooldownAtaqueTimer;
    public float dañoAtaque = 10f;
    public float tamañoAtaque = 0.5f; 
    public LayerMask capaEnemigos; 
    public Transform puntoArriba, puntoAbajo, puntoDerecha, puntoIzquierda;

    [Header("Sensores")]
    public Transform detectorSuelo;
    public float radioDeteccion = 0.2f;
    public LayerMask capaSuelo;
    private bool estaEnSuelo;
    public Transform detectorPared;
    public LayerMask capaPared;
    public float fuerzaSaltoParedX = 10f;
    public float fuerzaSaltoParedY = 12f;
    public float tiempoControlPared = 0.2f; 
    private bool tocandoPared;
    private float tiempoUltimoSaltoPared;

    [Header("Dash")]
    public float fuerzaDash = 20f;
    public float tiempoDashMax = 0.2f;   
    public float cooldownDashMax = 1f;   
    private float dashTimer;
    private float dashCooldownTimer;
    private bool estaHaciendoDash;
    private float gravedadOriginal;

    [Header("Knockback")]
    public float tiempoKnockback = 0.2f;
    private float knockbackTimer;

    private GameSceneManager gsm;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravedadOriginal = rb.gravityScale;
        saludActual = saludMaxima;
        if (gsm == null) gsm = FindFirstObjectByType<GameSceneManager>();
    }

    void Update()
    {
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (cooldownAtaqueTimer > 0) cooldownAtaqueTimer -= Time.deltaTime;
        if (timerInvencibilidad > 0) timerInvencibilidad -= Time.deltaTime;
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !estaHaciendoDash) EjecutarDash();

        if (estaHaciendoDash)
        {
            ActualizarDash();
            return; 
        }

        if (cooldownAtaqueTimer <= 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow)) Atacar("Arriba", puntoArriba);
            else if (Input.GetKeyDown(KeyCode.DownArrow)) Atacar("Abajo", puntoAbajo);
            else if (Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                transform.localScale = new Vector3(1, 1, 1); // Obligamos a mirar a la derecha
                Atacar("Derecha", puntoDerecha); 
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) 
            {
                transform.localScale = new Vector3(-1, 1, 1); // Obligamos a mirar a la izquierda
                Atacar("Izquierda", puntoDerecha); // Usamos puntoDerecha porque al girar, es el que queda al frente
            }
        }

        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);
        tocandoPared = Physics2D.OverlapCircle(detectorPared.position, radioDeteccion, capaPared);

        if (estaEnSuelo) coyoteTimer = tiempoCoyoteMax;
        else coyoteTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) bufferTimer = tiempoBufferMax;
        else bufferTimer -= Time.deltaTime;

        if (Time.time > tiempoUltimoSaltoPared + tiempoControlPared)
        {
            float entradaX = 0;
            if (Input.GetKey(KeyCode.A)) entradaX = -1;
            else if (Input.GetKey(KeyCode.D)) entradaX = 1;

            movimientoX = entradaX;
            rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
            GirarSprite();
        }

        if (bufferTimer > 0f)
        {
            if (coyoteTimer > 0f) Saltar();
            else if (tocandoPared) EjecutarWallJump();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) gsm.IrMenu();
    } 

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        coyoteTimer = 0f; 
        bufferTimer = 0f; 
    }

    /// <summary>
    /// Instancia el prefab de ataque en la posicion y rotacion del punto indicado.
    /// </summary>
    void Atacar(string direccion, Transform punto)
    {
        if (punto == null || prefabAtaque == null) return; 

        cooldownAtaqueTimer = tiempoEntreAtaques;

        // CAMBIO AQUÍ: Usamos 'punto.rotation' en lugar de 'Quaternion.identity'
        GameObject ataqueTemporal = Instantiate(prefabAtaque, punto.position, punto.rotation, punto);
        
        ataqueTemporal.transform.localScale = new Vector3(tamañoAtaque, tamañoAtaque, 1);

        Destroy(ataqueTemporal, tiempoVidaAtaque);

        // Detección física (se mantiene para lógica interna)
        Physics2D.OverlapBoxAll(punto.position, new Vector2(tamañoAtaque, tamañoAtaque), 0, capaEnemigos);
    }
    
    void EjecutarDash()
    {
        estaHaciendoDash = true;
        dashTimer = tiempoDashMax;
        dashCooldownTimer = cooldownDashMax;
        float direccionDash = transform.localScale.x > 0 ? 1 : -1;
        rb.linearVelocity = new Vector2(direccionDash * fuerzaDash, 0f);
        rb.gravityScale = 0;
    }

    void ActualizarDash()
    {
        dashTimer -= Time.deltaTime;
        float direccionDash = transform.localScale.x > 0 ? 1 : -1;
        rb.linearVelocity = new Vector2(direccionDash * fuerzaDash, 0f);

        if (dashTimer <= 0)
        {
            estaHaciendoDash = false;
            rb.gravityScale = gravedadOriginal;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void EjecutarWallJump()
    {
        tiempoUltimoSaltoPared = Time.time;
        float direccionSalto = transform.localScale.x > 0 ? -1 : 1;
        rb.linearVelocity = new Vector2(direccionSalto * fuerzaSaltoParedX, fuerzaSaltoParedY);
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
        bufferTimer = 0f; 
    }

    void GirarSprite()
    {
        // Calculamos si el ataque físico (el prefab) sigue existiendo
        bool ataqueActivo = cooldownAtaqueTimer > (tiempoEntreAtaques - tiempoVidaAtaque);
        
        // Si estamos atacando, impedimos que el movimiento gire al personaje
        if (ataqueActivo) return;

        if (movimientoX > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (movimientoX < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    public void ModificarSalud(float cantidad)
    {
        if (cantidad < 0 && timerInvencibilidad > 0) return;
        saludActual += cantidad;
        if (cantidad < 0) timerInvencibilidad = tiempoInvencibilidad;
        if (saludActual <= 0) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void RecibirDano(float cantidadDano, Vector2 posicionAtacante, float fuerzaEmpuje = 10f)
    {
        // if (saludActual <= 0)
        // {
        //     Debug.Log($"PLAYER: Sin vida, debería MORIR");
        // }
        // else
        // {
            saludActual -= cantidadDano;
            Debug.Log($"PLAYER: Vida restante de {saludActual} puntos");

            AplicarKnockback(posicionAtacante, fuerzaEmpuje);
        // }
    }

    private void AplicarKnockback(Vector2 posicionAtacante, float fuerzaEmpuje)
    {
        // Si no hay empuje no hace falta Knockback
        if (fuerzaEmpuje <= 0) return;

        knockbackTimer = tiempoKnockback;

        float direccionAtaqueX = transform.position.x > posicionAtacante.x ? 1 : -1;

        Vector2 direccionEmpuje = new Vector2(direccionAtaqueX, 0.5f).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = direccionEmpuje * fuerzaEmpuje;
    }

    private void OnDrawGizmosSelected()
    {
        if (detectorSuelo) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion); }
        if (detectorPared) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(detectorPared.position, radioDeteccion); }
        Gizmos.color = Color.yellow;
        if (puntoArriba) Gizmos.DrawWireCube(puntoArriba.position, new Vector3(tamañoAtaque, tamañoAtaque, 0));
        if (puntoAbajo) Gizmos.DrawWireCube(puntoAbajo.position, new Vector3(tamañoAtaque, tamañoAtaque, 0));
        if (puntoDerecha) Gizmos.DrawWireCube(puntoDerecha.position, new Vector3(tamañoAtaque, tamañoAtaque, 0));
        if (puntoIzquierda) Gizmos.DrawWireCube(puntoIzquierda.position, new Vector3(tamañoAtaque, tamañoAtaque, 0));
    }
}