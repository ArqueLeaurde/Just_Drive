using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic; // Nécessaire pour List

public class TimeManager : MonoBehaviour
{
    // --- Classe pour définir les périodes de la journée ---
    [System.Serializable]
    public class TimePeriod
    {
        public string name;
        public float startTime; // Heure de début (0-24)
        public Texture2D skyboxTexture;
        public LightShadows shadowType;
    }

    [Header("Durée du Cycle")]
    [Tooltip("Durée totale d'une journée en secondes réelles.")]
    [SerializeField] private float dayDurationInSeconds = 120f; // 2 minutes pour un cycle complet

    [Header("Éléments à contrôler")]
    [SerializeField] private Light globalLight;
    [SerializeField] private List<TimePeriod> periods; // Liste des périodes (Nuit, Jour, etc.)

    [Header("Courbes et Dégradés")]
    [Tooltip("Couleur de la lumière directionnelle au cours de la journée.")]
    [SerializeField] private Gradient lightColorGradient;
    [Tooltip("Intensité de la lumière directionnelle au cours de la journée.")]
    [SerializeField] private AnimationCurve lightIntensityCurve;

    [Header("État Actuel (Debug)")]
    [Range(0, 24)]
    [SerializeField] private float timeOfDay;
    private int dayCount;

    private Material skyboxMaterial;

    private void Start()
    {
        // On clone le material de la skybox pour ne pas modifier l'asset de base
        skyboxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyboxMaterial;

        // Trie les périodes par heure de début pour s'assurer qu'elles sont dans l'ordre
        periods.Sort((a, b) => a.startTime.CompareTo(b.startTime));

        // Force la mise à jour de l'environnement au démarrage
        UpdateEnvironment(timeOfDay);
    }

    private void Update()
    {
        // Fait avancer le temps
        // 24 heures en 'dayDurationInSeconds'
        timeOfDay += (Time.deltaTime / dayDurationInSeconds) * 24f;

        // Gère le passage à un nouveau jour
        if (timeOfDay >= 24f)
        {
            timeOfDay -= 24f;
            dayCount++;
        }

        // Met à jour la lumière et la skybox en continu
        UpdateEnvironment(timeOfDay);
    }

    private void UpdateEnvironment(float currentTime)
    {
        // --- Mise à jour de la lumière ---
        float timeFraction = currentTime / 24f; // Temps normalisé (0-1)
        globalLight.color = lightColorGradient.Evaluate(timeFraction);
        globalLight.intensity = lightIntensityCurve.Evaluate(timeFraction);
        RenderSettings.fogColor = globalLight.color;

        // --- Mise à jour de la skybox ---
        // Trouve la période actuelle et la suivante
        TimePeriod currentPeriod = GetCurrentPeriod(currentTime);
        TimePeriod nextPeriod = GetNextPeriod(currentTime);

        // Calcule le facteur de mélange (blend) entre les deux skyboxes
        float blendFactor = Mathf.InverseLerp(currentPeriod.startTime, nextPeriod.startTime, currentTime);
        // Gère le cas où on passe de la dernière période à la première (ex: de nuit à nuit)
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

        // Met à jour les ombres
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
        // Si aucune n'est trouvée (ex: avant la première heure), on renvoie la dernière
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
        // Si on est après la dernière période, la prochaine est la première
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