using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public float velocidadCaminar = 5f;
    public float velocidadCorrer = 9f;
    public float fuerzaSalto = 12f;

    
    public Transform comprobadorSuelo; 
    public float radioDeteccion = 0.1f;
    public LayerMask capaSuelo; 

    private Rigidbody2D rb;
    private bool estaEnSuelo;
    private float movimientoHorizontal;

    void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        
        float velocidadActual = Input.GetKey(KeyCode.LeftShift) ? velocidadCorrer : velocidadCaminar;
        
        //velocidad al Rigidbody
        rb.linearVelocity = new Vector2(movimientoHorizontal * velocidadActual, rb.linearVelocity.y);

       
        estaEnSuelo = Physics2D.OverlapCircle(comprobadorSuelo.position, radioDeteccion, capaSuelo);

        // 4. Salto
        if (Input.GetButtonDown("Jump") && estaEnSuelo)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }

        // Girar el cubo según la dirección
        GirarPersonaje(movimientoHorizontal);
    }

    void GirarPersonaje(float direccion)
    {
        if (direccion > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direccion < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    
  private void OnDrawGizmos()
    {
        if (comprobadorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(comprobadorSuelo.position, 0.2f);
        }
    }
    }
    
