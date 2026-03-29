using UnityEngine;
//prueba git
public class MovimientoCaballero : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;
    private Rigidbody2D rb;
    private float movimientoX;

    [Header("Coyote Time")]
    public float tiempoCoyoteMax = 0.15f; 
    private float coyoteTimer;

    [Header("Jump Buffer")] // <-- NUEVO: Sección para el Buffer
    public float tiempoBufferMax = 0.1f;
    private float bufferTimer;

    [Header("Detección de Suelo")]
    public Transform detectorSuelo;
    public float radioDeteccion = 0.2f;
    public LayerMask capaSuelo;
    private bool estaEnSuelo;

    [Header("Wall Jump")]
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

    private GameSceneManager gsm;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gravedadOriginal = rb.gravityScale;

        if (gsm == null)
        {
            gsm = FindFirstObjectByType<GameSceneManager>();
        }
    }

    void Update()
    {
        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0 && !estaHaciendoDash)
        {
            estaHaciendoDash = true;
            dashTimer = tiempoDashMax;
            dashCooldownTimer = cooldownDashMax;
            
            float direccionDash = transform.localScale.x > 0 ? 1 : -1;
            rb.linearVelocity = new Vector2(direccionDash * fuerzaDash, 0f);
            rb.gravityScale = 0;
        }

        if (estaHaciendoDash)
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
            return; 
        }

        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);
        tocandoPared = Physics2D.OverlapCircle(detectorPared.position, radioDeteccion, capaPared);

        // Lógica de gestión del temporizador Coyote
        if (estaEnSuelo)
        {
            coyoteTimer = tiempoCoyoteMax;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // --- NUEVA LÓGICA: Gestión del Buffer de Salto ---
        if (Input.GetButtonDown("Jump"))
        {
            bufferTimer = tiempoBufferMax; // "Guardamos" la intención de saltar
        }
        else
        {
            bufferTimer -= Time.deltaTime;
        }
        // ------------------------------------------------

        if (Time.time > tiempoUltimoSaltoPared + tiempoControlPared)
        {
            movimientoX = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
            GirarSprite();
        }

        // Lógica de salto mejorada con Coyote y Buffer
        if (bufferTimer > 0f) // Si el jugador ha pulsado saltar hace muy poco...
        {
            if (coyoteTimer > 0f) // ...y todavía tiene tiempo de Coyote (está en suelo o casi)
            {
                Saltar();
            }
            else if (tocandoPared) // O si está tocando pared
            {
                EjecutarWallJump();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gsm.IrMenu();
        }
    } 

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        coyoteTimer = 0f; 
        bufferTimer = 0f; // Vaciamos el buffer tras saltar para no repetir el salto
    }

    void EjecutarWallJump()
    {
        tiempoUltimoSaltoPared = Time.time;
        float direccionSalto = transform.localScale.x > 0 ? -1 : 1;
        rb.linearVelocity = new Vector2(direccionSalto * fuerzaSaltoParedX, fuerzaSaltoParedY);

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
        
        bufferTimer = 0f; // Vaciamos el buffer tras el wall jump
    }

    void GirarSprite()
    {
        if (movimientoX > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (movimientoX < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void OnDrawGizmosSelected()
    {
        if (detectorSuelo) { Gizmos.color = Color.red; Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion); }
        if (detectorPared) { Gizmos.color = Color.blue; Gizmos.DrawWireSphere(detectorPared.position, radioDeteccion); }
    }
}