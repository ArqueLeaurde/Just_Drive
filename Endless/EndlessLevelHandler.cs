using UnityEngine;
using System.Collections;


public class EndlessLevelHandler : MonoBehaviour
{
    [SerializeField]
    GameObject[] sectionsPrefabs;

    GameObject[] sectionsPool = new GameObject[20];

    GameObject[] sections = new GameObject[10];

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    const float sectionLength = 26;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Track the player
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int prefabIndex = 0;

        //Create a pool for the endless section
        for (int i = 0; i < sectionsPool.Length; i++)
        {
            sectionsPool[i] = Instantiate(sectionsPrefabs[prefabIndex]);
            sectionsPool[i].SetActive(false);

            prefabIndex++;

            //Loop the index if out of prefab
            if (prefabIndex > sectionsPrefabs.Length - 1)
                prefabIndex = 0;
        }

        //Add first section to the road
        for (int i = 0;i < sections.Length; i++)
        {
            //Get a random section
            GameObject randomSection = GetRandomSectionFromPool();

            // Move it into position and set it to active
            randomSection.transform.position = new Vector3(sectionsPool[i].transform.position.x, -10, i * sectionLength);
            randomSection.SetActive(true);

            //Set the sectionb in the array
            sections[i] = randomSection;
        }

        StartCoroutine(UpdateLessOftenCO());
        
    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            UpdateSectionPositions();
            yield return waitFor100ms;
        }
    }

    void UpdateSectionPositions()
    {
        for (int i = 0; i< sections.Length; i++)
        {
            //Check if a section is too far behind
            if (sections[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
            {
                //Store the position of the section and disable it
                Vector3 lastSectionPosition = sections[i].transform.position;
                sections[i].SetActive(false);

                //Get a new section move it forward and enable it
                sections[i] = GetRandomSectionFromPool();

                //Move The new section and activate it
                sections[i].transform.position = new Vector3(lastSectionPosition.x, -10, lastSectionPosition.z + sectionLength * sections.Length);
                sections[i].SetActive(true);
            }

        }
    }

    GameObject GetRandomSectionFromPool()
    {
        //Pick a random index and hope that it's available
        int randomIndex = Random.Range(0, sectionsPool.Length);

        bool isNewSectionFound = false;

        while(!isNewSectionFound)
        {
            // Check if the section is active
            if (!sectionsPool[randomIndex].activeInHierarchy)
                isNewSectionFound = true;
            else
            {   
                //If it was active we need to find another one
                randomIndex++;

                //ensure that we loop around if we reach the end of array
                if (randomIndex > sectionsPool.Length - 1)
                    randomIndex = 0;
            }
        }

        return sectionsPool[randomIndex];
    }

}
