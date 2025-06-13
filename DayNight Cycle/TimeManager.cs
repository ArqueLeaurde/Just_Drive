using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // N�cessaire pour List

public class TimeManager : MonoBehaviour
{
    // --- Classe pour d�finir les p�riodes de la journ�e ---
    [System.Serializable]
    public class TimePeriod
    {
        public string name;
        public float startTime; // Heure de d�but (0-24)
        public Texture2D skyboxTexture;
        public LightShadows shadowType;
    }

    [Header("Dur�e du Cycle")]
    [Tooltip("Dur�e totale d'une journ�e en secondes r�elles.")]
    [SerializeField] private float dayDurationInSeconds = 120f; // 2 minutes pour un cycle complet

    [Header("�l�ments � contr�ler")]
    [SerializeField] private Light globalLight;
    [SerializeField] private List<TimePeriod> periods; // Liste des p�riodes (Nuit, Jour, etc.)

    [Header("Courbes et D�grad�s")]
    [Tooltip("Couleur de la lumi�re directionnelle au cours de la journ�e.")]
    [SerializeField] private Gradient lightColorGradient;
    [Tooltip("Intensit� de la lumi�re directionnelle au cours de la journ�e.")]
    [SerializeField] private AnimationCurve lightIntensityCurve;

    [Header("�tat Actuel (Debug)")]
    [Range(0, 24)]
    [SerializeField] private float timeOfDay;
    private int dayCount;

    private Material skyboxMaterial;

    private void Start()
    {
        // On clone le material de la skybox pour ne pas modifier l'asset de base
        skyboxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyboxMaterial;

        // Trie les p�riodes par heure de d�but pour s'assurer qu'elles sont dans l'ordre
        periods.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        // Force la mise � jour de l'environnement au d�marrage
        UpdateEnvironment(timeOfDay);
    }

    private void Update()
    {
        // Fait avancer le temps
        // 24 heures en 'dayDurationInSeconds'
        timeOfDay += (Time.deltaTime / dayDurationInSeconds) * 24f;

        // G�re le passage � un nouveau jour
        if (timeOfDay >= 24f)
        {
            timeOfDay -= 24f;
            dayCount++;
        }

        // Met � jour la lumi�re et la skybox en continu
        UpdateEnvironment(timeOfDay);
    }

    private void UpdateEnvironment(float currentTime)
    {
        // --- Mise � jour de la lumi�re ---
        float timeFraction = currentTime / 24f; // Temps normalis� (0-1)
        globalLight.color = lightColorGradient.Evaluate(timeFraction);
        globalLight.intensity = lightIntensityCurve.Evaluate(timeFraction);
        RenderSettings.fogColor = globalLight.color;

        // --- Mise � jour de la skybox ---
        // Trouve la p�riode actuelle et la suivante
        TimePeriod currentPeriod = GetCurrentPeriod(currentTime);
        TimePeriod nextPeriod = GetNextPeriod(currentTime);

        // Calcule le facteur de m�lange (blend) entre les deux skyboxes
        float blendFactor = Mathf.InverseLerp(currentPeriod.startTime, nextPeriod.startTime, currentTime);
        // G�re le cas o� on passe de la derni�re p�riode � la premi�re (ex: de nuit � nuit)
        if (currentPeriod.startTime > nextPeriod.startTime)
        {
            float total = (24f - currentPeriod.startTime) + nextPeriod.startTime;
            float current = currentTime - currentPeriod.startTime;
            if (current < 0) current += 24f;
            blendFactor = current / total;
        }

        skyboxMaterial.SetTexture("_Texture1", currentPeriod.skyboxTexture);
        skyboxMaterial.SetTexture("_Texture2", nextPeriod.skyboxTexture);
        skyboxMaterial.SetFloat("_Blend", blendFactor);

        // Met � jour les ombres
        globalLight.shadows = currentPeriod.shadowType;
    }

    private TimePeriod GetCurrentPeriod(float currentTime)
    {
        for (int i = periods.Count - 1; i >= 0; i--)
        {
            if (currentTime >= periods[i].startTime)
            {
                return periods[i];
            }
        }
        // Si aucune n'est trouv�e (ex: avant la premi�re heure), on renvoie la derni�re
        return periods[periods.Count - 1];
    }

    private TimePeriod GetNextPeriod(float currentTime)
    {
        for (int i = 0; i < periods.Count; i++)
        {
            if (currentTime < periods[i].startTime)
            {
                return periods[i];
            }
        }
        // Si on est apr�s la derni�re p�riode, la prochaine est la premi�re
        return periods[0];
    }

    // Permet de changer l'heure manuellement depuis un slider dans l'inspecteur
    private void OnValidate()
    {
        if (globalLight != null && periods != null && periods.Count > 0 && skyboxMaterial != null)
        {
            UpdateEnvironment(timeOfDay);
        }
    }
}