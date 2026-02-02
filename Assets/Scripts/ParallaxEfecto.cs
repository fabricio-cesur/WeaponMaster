using UnityEngine;

public class ParallaxEfecto : MonoBehaviour
{
    public Transform camara;     
    public float multiplicadorParallax; 

    private Vector3 ultimaPosicionCamara;

    void Start()
    {
        ultimaPosicionCamara = camara.position;
    }

    void LateUpdate()
    {
        
        Vector3 movimientoCamara = camara.position - ultimaPosicionCamara;
        
        
        transform.position += new Vector3(movimientoCamara.x * multiplicadorParallax, 0, 0);
        
        ultimaPosicionCamara = camara.position;
    }
}