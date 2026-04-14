using UnityEngine;

public class Pinchos : MonoBehaviour, IDamageable
{
    [SerializeField] private float danoPincho = 1;
    public float fuerzaEmpuje = 15f;

    private void OnCollisionEnter2D(Collision2D otro)
    {
        GameObject otroObjeto = otro.gameObject;
        string tag = otroObjeto.tag;
        
        if (otroObjeto.GetComponent<IDamageable>() != null)
        {
            IDamageable damageable = otroObjeto.GetComponent<IDamageable>();
            Debug.Log($"PINCHOS: Daño a {otro.gameObject.name} de {danoPincho} puntos.");
            damageable.RecibirDano(danoPincho, transform.position, fuerzaEmpuje);
        }
    }

    public void RecibirDano(float cantidadDano, Vector2 posicionAtacante, float fuerzaEmpuje = 10f)
    {
    }
}
