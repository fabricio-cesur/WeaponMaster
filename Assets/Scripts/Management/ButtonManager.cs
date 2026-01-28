using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public void BotonJugar()
    {
        GameSceneManager.instance.IrJuego(); 
    }

    public void BotonSalir()
    {
        GameSceneManager.instance.SalirDelJuego();
    }
}