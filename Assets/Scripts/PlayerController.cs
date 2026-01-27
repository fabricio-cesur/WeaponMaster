using UnityEngine;

public class MovimientoCaballero : MonoBehaviour
{
    [Header("Movimiento Base")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;
    private Rigidbody2D rb;
    private float movimientoX;

    [Header("Detección de Suelo")]
    public Transform detectorSuelo;
    public float radioDeteccion = 0.2f;
    public LayerMask capaSuelo;
    private bool estaEnSuelo;

    [Header("Salto de Pared (Wall Jump)")]
    public Transform detectorPared;
    public LayerMask capaPared;
    public float fuerzaSaltoParedX = 10f;
    public float fuerzaSaltoParedY = 12f;
    public float tiempoControlPared = 0.2f; 
    private bool tocandoPared;
    private float tiempoUltimoSaltoPared;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
        estaEnSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);
        tocandoPared = Physics2D.OverlapCircle(detectorPared.position, radioDeteccion, capaPared);

        
        if (Time.time > tiempoUltimoSaltoPared + tiempoControlPared)
        {
            movimientoX = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
            GirarSprite();
        }

        
        if (Input.GetButtonDown("Jump"))
        {
            if (estaEnSuelo)
            {
                Saltar();
            }
            else if (tocandoPared)
            {
                EjecutarWallJump();
            }
        }
    }

    void Saltar()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
    }

    void EjecutarWallJump()
    {
        tiempoUltimoSaltoPared = Time.time;
        
        
        float direccionSalto = transform.localScale.x > 0 ? -1 : 1;
        
        
        rb.linearVelocity = new Vector2(direccionSalto * fuerzaSaltoParedX, fuerzaSaltoParedY);

        
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void GirarSprite()
    {
        if (movimientoX > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (movimientoX < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    
    private void OnDrawGizmosSelected()
    {
        if (detectorSuelo) Gizmos.color = Color.red; Gizmos.DrawWireSphere(detectorSuelo.position, radioDeteccion);
        if (detectorPared) Gizmos.color = Color.blue; Gizmos.DrawWireSphere(detectorPared.position, radioDeteccion);
    }
}