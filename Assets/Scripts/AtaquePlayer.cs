using UnityEngine;

public class AtaquePlayer : MonoBehaviour
{
    [SerializeField] private float danoAtaquePlayer;
    [SerializeField] private float empujeAtaquePlayer;

    private Transform transformJugador;
    private PlayerController scriptPlayerController;
    private GameObject jugador;

    void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        scriptPlayerController = FindFirstObjectByType<PlayerController>();

        if (jugador != null)
        {
            transformJugador = jugador.transform;
        }
    }

    public void ActualizarAtaque(float dano, float empuje)
    {
        danoAtaquePlayer = dano;
        empujeAtaquePlayer = empuje;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        GameObject otroObjeto = otro.gameObject;
        string tag = otroObjeto.tag;
        
        if (otroObjeto.GetComponent<IDamageable>() != null)
        {
            IDamageable damageable = otroObjeto.GetComponent<IDamageable>();
            Debug.Log($"PLAYER: Daño a {otro.gameObject.name} ({tag}) de {danoAtaquePlayer} puntos.");
            damageable.RecibirDano(danoAtaquePlayer, transformJugador.position, empujeAtaquePlayer);

            if (scriptPlayerController != null)
            {
                // AÑADIDO: Sonido al golpear enemigo
                scriptPlayerController.ReproducirSonidoImpacto("Enemigo");
                scriptPlayerController.AplicarRecoilAtaque();
            }
        }
        // AÑADIDO: Detección de colisión con Pinchos
        else if (otroObjeto.CompareTag("Pinchos"))
        {
            if (scriptPlayerController != null)
            {
                // AÑADIDO: Sonido al golpear pinchos y aplicar salto pogo
                scriptPlayerController.ReproducirSonidoImpacto("Pinchos");
                scriptPlayerController.AplicarRecoilAtaque();
            }
        }
    }
}