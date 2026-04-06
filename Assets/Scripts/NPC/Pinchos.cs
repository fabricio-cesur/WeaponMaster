using UnityEngine;

public class Pinchos : MonoBehaviour
{
    [SerializeField] private float danoPincho = 10;

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Player"))
        {
            IDamageable damageable = otro.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.RecibirDano(danoPincho);
            }
        }
    }
}
