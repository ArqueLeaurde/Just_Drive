using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomizeObjectis : MonoBehaviour
{

    [SerializeField]
    Vector3 localRotationMin = Vector3.zero;
    [SerializeField]
    Vector3 localRotationMax = Vector3.zero;

    [SerializeField]
    float localScaleMultiplierMin = 0.8f;
    [SerializeField]
    float localScaleMultiplierMax = 1.5f;

    // Ensure that we store the original scale so that elements don't grow/shrink too much if unlucky due to random
    Vector3 localScaleOriginal = Vector3.one;

    void Start()
    {
        localScaleOriginal = transform.localScale;
    }

    // OnEnable is called whenever the object shows up and is active
    void OnEnable()
    {

        //Set random rotation and scale between fixed values for items
        transform.localRotation = Quaternion.Euler(Random.Range(localRotationMin.x, localRotationMax.x), Random.Range(localRotationMin.y, localRotationMax.y), Random.Range(localRotationMin.z, localRotationMax.z));

        transform.localScale = localScaleOriginal * Random.Range(localScaleMultiplierMin, localScaleMultiplierMax);
    }

}
