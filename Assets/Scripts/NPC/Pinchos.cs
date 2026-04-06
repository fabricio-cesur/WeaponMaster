using UnityEngine;

public class Pinchos : MonoBehaviour
{
    [SerializeField] private float danoPincho = 10;

    private void OnCollisionEnter2D(Collision2D otro)
    {
        if (otro.gameObject.CompareTag("Player"))
        {

            Debug.Log($"PINCHOS: Toqué al jugador.");
            IDamageable damageable = otro.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log($"PINCHOS: Daño a jugador de {danoPincho} puntos.");
                damageable.RecibirDano(danoPincho);
            }
        }
    }
}
