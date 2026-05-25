using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    [Header("Configuración del Audio")]
    [Tooltip("Arrastra aquí tu MainMixer desde la carpeta del proyecto")]
    public AudioMixer mainMixer;
    
    [Header("Sliders de la UI")]
    public Slider sliderMusic;
    public Slider sliderSFX;

    void Start()
    {
        // 1. Cargamos los valores de PlayerPrefs (o 0.75 por defecto si es la primera vez)
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

        // 2. Preparamos el Slider de Música
        if (sliderMusic != null)
        {
            sliderMusic.value = savedMusicVol;
            // Esto hace que cada vez que muevas el slider, se llame a la función automáticamente
            sliderMusic.onValueChanged.AddListener(SetMusicVolume); 
        }

        // 3. Preparamos el Slider de Efectos (SFX)
        if (sliderSFX != null)
        {
            sliderSFX.value = savedSFXVol;
            sliderSFX.onValueChanged.AddListener(SetSFXVolume);
        }

        // 4. Aplicamos el volumen real nada más cargar el menú
        SetMusicVolume(savedMusicVol);
        SetSFXVolume(savedSFXVol);
    }

    public void SetMusicVolume(float sliderValue)
    {
        // Seguridad: Evitamos el 0 absoluto porque el logaritmo de 0 da error matemático
        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);

        // Convertimos el valor del slider (0 al 1) a decibelios (-80 a 0) usando una fórmula logarítmica
        mainMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        
        // Guardamos la preferencia del jugador
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);

        mainMixer.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
        PlayerPrefs.Save();
    }
}