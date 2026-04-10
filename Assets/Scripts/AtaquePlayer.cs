using UnityEngine;

public class AtaquePlayer : MonoBehaviour
{
    [SerializeField] private float danoAtaquePlayer = 30;

    private void OnTriggerEnter2D(Collider2D otro)
    {
        GameObject otroObjeto = otro.gameObject;
        string tag = otroObjeto.tag;
        
        if (otroObjeto.GetComponent<IDamageable>() != null)
        {
            IDamageable damageable = otroObjeto.GetComponent<IDamageable>();
            Debug.Log($"PLAYER: Daño a {otro.gameObject.name} ({tag}) de {danoAtaquePlayer} puntos.");
            damageable.RecibirDano(danoAtaquePlayer);
        }
    }
}
