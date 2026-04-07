using UnityEngine;

public class Pinchos : MonoBehaviour
{
    [SerializeField] private float danoPincho = 10;

    private void OnCollisionEnter2D(Collision2D otro)
    {
        GameObject otroObjeto = otro.gameObject;
        string tag = otroObjeto.tag;
        
        if (otroObjeto.GetComponent<IDamageable>() != null)
        {
            IDamageable damageable = otroObjeto.GetComponent<IDamageable>();
            Debug.Log($"PINCHOS: Daño a {otro.gameObject.name} de {danoPincho} puntos.");
            damageable.RecibirDano(danoPincho);
        }
        // if (tag == "Player" || tag == "Enemy")
        // {
        //     IDamageable damageable = otroObjeto.GetComponent<IDamageable>();
        //     if (damageable != null)
        //     {
        //         Debug.Log($"PINCHOS: Daño a {otro.gameObject.name} de {danoPincho} puntos.");
        //         damageable.RecibirDano(danoPincho);
        //     }
        // }
    }
}
