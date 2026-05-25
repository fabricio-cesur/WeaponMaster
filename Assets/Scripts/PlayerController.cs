using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Salud")]
    public float saludMaxima = 5f;
    public float saludActual;
    public float tiempoInvencibilidad = 1f;
    private float timerInvencibilidad;
    private bool estaMuerto = false;

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

    [Header("Audio SFX Dinámico")]
    [Tooltip("Sonido que suena siempre que la espada corta el aire")]
    public AudioClip sonidoAtaqueAire;
    [Tooltip("Sonido cuando golpeas a un enemigo")]
    public AudioClip sonidoImpactoEnemigo;
    [Tooltip("Sonido cuando golpeas pinchos u obstáculos")]
    public AudioClip sonidoImpactoPinchos;
    private AudioSource audioSource;

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
    public float fuerzaEmpujeAtaque = 10f;
    public float tiempoKnockback = 0.2f;
    private float knockbackTimer;

    [Header("Recoil")]
    public float fuerzaRecoilArriba = 15f;
    public float fuerzaRecoilLateral = 10f;
    private string ultimaDireccionAtaque;
    public float tiempoRecoilLateral = 0.1f;
    private float recoilTimer;

    private GameSceneManager gsm;
    private GameManager gm;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // INTEGRACIÓN: Buscamos el componente de sonido
        estaMuerto = false;

        gm = GameManager.gm;

        rb = GetComponent<Rigidbody2D>();
        gravedadOriginal = rb.gravityScale;

        if (gm != null && gm.tieneDatos)
        {
            transform.position = gm.posicionJugador;
            saludActual = gm.saludJugador;
        }
        else
        {
            saludActual = saludMaxima;
        }

        if (gsm == null) gsm = FindFirstObjectByType<GameSceneManager>();
    }

    void Update()
    {
        if (estaMuerto)
        {
            if (knockbackTimer > 0)
            {
                knockbackTimer -= Time.deltaTime;
            }
            else
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            }
            return; 
        }

        if (dashCooldownTimer > 0) dashCooldownTimer -= Time.deltaTime;
        if (cooldownAtaqueTimer > 0) cooldownAtaqueTimer -= Time.deltaTime;
        if (timerInvencibilidad > 0) timerInvencibilidad -= Time.deltaTime;
        
        if (recoilTimer > 0) recoilTimer -= Time.deltaTime; 

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
            else if (Input.GetKeyDown(KeyCode.RightArrow)) Atacar("Derecha", puntoDerecha); 
            // SOLUCIÓN: Usamos puntoDerecha aquí porque cuando la escala cambia a -1, este rota físicamente a la izquierda de forma automática.
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) Atacar("Izquierda", puntoDerecha); 
        }

        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);
        tocandoPared = Physics2D.OverlapCircle(detectorPared.position, radioDeteccion, capaPared);

        if (estaEnSuelo) coyoteTimer = tiempoCoyoteMax;
        else coyoteTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) bufferTimer = tiempoBufferMax;
        else bufferTimer -= Time.deltaTime;

        // --- MOVIMIENTO HORIZONTAL ---
        if (Time.time > tiempoUltimoSaltoPared + tiempoControlPared && recoilTimer <= 0 && !EstaAtacando())
        {
            float entradaX = 0;
            if (Input.GetKey(KeyCode.A)) entradaX = -1;
            else if (Input.GetKey(KeyCode.D)) entradaX = 1;

            movimientoX = entradaX;
            rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
            GirarSprite();
        }
        else if (EstaAtacando() && recoilTimer <= 0 && estaEnSuelo)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (bufferTimer > 0f)
        {
            if (coyoteTimer > 0f) Saltar();
            else if (tocandoPared) EjecutarWallJump();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gm != null)
            {
                gm.posicionJugador = transform.position;
                gm.saludJugador = saludActual;
                gm.tieneDatos = true;

                gm.GuardarPartidaEnDisco();
            }
            gsm.IrMenu();
        }
    }

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        coyoteTimer = 0f; 
        bufferTimer = 0f; 
    }

    bool EstaAtacando()
    {
        return cooldownAtaqueTimer > (tiempoEntreAtaques - tiempoVidaAtaque);
    }

    void Atacar(string direccion, Transform punto)
    {
        ultimaDireccionAtaque = direccion;
        if (punto == null || prefabAtaque == null) return; 

        cooldownAtaqueTimer = tiempoEntreAtaques;

        if (direccion == "Derecha")
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direccion == "Izquierda")
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

        ReproducirSonido(sonidoAtaqueAire);

        if (animator != null)
        {
            if (direccion == "Derecha" || direccion == "Izquierda")
            {
                animator.SetTrigger("AtacarLateral");
            }
            else if (direccion == "Arriba")
            {
                animator.SetTrigger("AtacarArriba");
            }
            else if (direccion == "Abajo")
            {
                animator.SetTrigger("AtacarAbajo");
            }
        }

        GameObject ataqueTemporal = Instantiate(prefabAtaque, punto.position, punto.rotation, punto);

        // --- NUEVA LÓGICA DE ESCALA RECTANGULAR ---
        float largo = tamañoAtaque; 
        float grosor = tamañoAtaque * 0.6f; // El grosor será menos de la mitad que el largo. Cambia el 0.4f a tu gusto.

        if (direccion == "Arriba" || direccion == "Abajo")
        {
            // Ataque recostado: mucho alcance horizontal (X), poco alcance vertical (Y)
            ataqueTemporal.transform.localScale = new Vector3(largo, grosor, 1);
        }
        else
        {
            // Ataque de pie: poco alcance horizontal (X), mucho alcance vertical (Y)
            ataqueTemporal.transform.localScale = new Vector3(grosor, largo, 1);
        }

        if (ataqueTemporal.TryGetComponent(out AtaquePlayer scriptAtaque))
        {
            scriptAtaque.ActualizarAtaque(dañoAtaque, fuerzaEmpujeAtaque);
        }

        Destroy(ataqueTemporal, tiempoVidaAtaque);
        
        // He borrado la línea de Physics2D.OverlapBoxAll porque no estaba guardando 
        // la información en ninguna variable y solo consumía recursos innecesarios.
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
        // CAMBIO CLAVE: Bloqueamos el re-giro por movimiento mientras dure todo el cooldown del ataque (0.3s)
        if (cooldownAtaqueTimer > 0) return;

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
        saludActual -= cantidadDano;
        AplicarKnockback(posicionAtacante, fuerzaEmpuje);
        if (saludActual <= 0)
        {
            Morir();
        }
        else
        {
            Debug.Log($"PLAYER: Vida restante de {saludActual} puntos");
        }
    }

    private void AplicarKnockback(Vector2 posicionAtacante, float fuerzaEmpuje)
    {
        if (fuerzaEmpuje <= 0) return;

        knockbackTimer = tiempoKnockback;

        float direccionAtaqueX = transform.position.x > posicionAtacante.x ? 1 : -1;

        Vector2 direccionEmpuje = new Vector2(direccionAtaqueX, 0.5f).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = direccionEmpuje * fuerzaEmpuje;
    }

    private void Morir()
    {
        estaMuerto = true;
        animator.SetBool("estaMuerto", true);
        
        if (gm != null)
        {
            gm.ReiniciarDatosGuardados();
            gm.saludJugador = saludMaxima;
        }

        Invoke(nameof(ReiniciarEscena), 1f);
    }

    private void ReiniciarEscena()
    {
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual);
    }

    public void AplicarRecoilAtaque()
    {
        rb.linearVelocity = Vector2.zero;

        switch (ultimaDireccionAtaque)
        {
            case "Abajo":
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaRecoilArriba);
                break;

            case "Derecha":
                rb.linearVelocity = new Vector2(-fuerzaRecoilLateral, rb.linearVelocity.y);
                break;

            case "Izquierda":
                rb.linearVelocity = new Vector2(fuerzaRecoilLateral, rb.linearVelocity.y);
                break;

            case "Arriba":
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -fuerzaRecoilArriba);
                break;
        }
    }

    public void ReproducirSonidoImpacto(string tipoDeImpacto)
    {
        if (tipoDeImpacto == "Enemigo")
        {
            ReproducirSonido(sonidoImpactoEnemigo);
        }
        else if (tipoDeImpacto == "Pinchos")
        {
            ReproducirSonido(sonidoImpactoPinchos);
        }
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
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