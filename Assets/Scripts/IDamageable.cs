using UnityEngine;

public interface IDamageable
{
    void RecibirDano(float cantidadDano, Vector2 posicionAtacante, float fuerzaEmpuje = 10f);
}
