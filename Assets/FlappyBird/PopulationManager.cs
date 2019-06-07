using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FlappyBird
{
    public class PopulationManager : MonoBehaviour
    {
        // Serializable fields

        [SerializeField]
        private GameObject m_BirdPrefab = null;
        [SerializeField]
        private int m_PopulationSize = 50;
        [SerializeField]
        private float m_TrialTime = 5f;
        [SerializeField]
        private bool m_EnableMutation = false;
        [SerializeField]
        private float m_MutationProbability = 0f;
        [SerializeField]
        private bool m_RandomlyOffsetSpawnPosition = false;
        [SerializeField]
        private float m_SpawnPositionRadius = 2f;

        // Fields

        private List<GameObject> m_Population = new List<GameObject>();

        private float m_ElapsedTime = 0f;
        private int m_GenerationIndex = 0;

        // MonoBehaviour's interface

        private void OnGUI()
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 25;
            guiStyle.normal.textColor = Color.white;

            GUI.BeginGroup(new Rect(10, 10, 250, 150));
            GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
            GUI.Label(new Rect(10, 25, 200, 30), "Generation: " + (m_GenerationIndex + 1), guiStyle);
            GUI.Label(new Rect(10, 50, 200, 30), "Time: " + m_ElapsedTime.ToString("F2"), guiStyle);
            GUI.Label(new Rect(10, 75, 200, 30), "Population: " + m_Population.Count, guiStyle);
            GUI.Label(new Rect(10, 100, 200, 30), "Living: " + GetLivingBotCount(), guiStyle);
            GUI.EndGroup();
        }

        private void Start()
        {
            for (int index = 0; index < m_PopulationSize; ++index)
            {
                GameObject botInstance = SpawnBird();

                Brain botBrain = botInstance.GetComponent<Brain>();

                if (botBrain == null)
                {
                    Destroy(botInstance);
                    continue;
                }

                botInstance.name = "Agent " + (index + 1).ToString();

                botBrain.Init(true);
                m_Population.Add(botInstance);
            }
        }

        private void Update()
        {
            m_ElapsedTime += Time.deltaTime;

            if (m_ElapsedTime >= m_TrialTime)
            {
                StartNewTrial();
            }
            else
            {
                if (AreAllBirdsDead() || Input.GetKeyDown(KeyCode.N))
                {
                    StartNewTrial();
                }
            }
        }

        // INTERNALS

        private GameObject Breed(GameObject i_FirstParent, GameObject i_SecondParent)
        {
            if (i_FirstParent == null || i_SecondParent == null)
            {
                return null;
            }

            Brain firstParentBrain = i_FirstParent.GetComponent<Brain>();
            Brain secondParentBrain = i_SecondParent.GetComponent<Brain>();

            if (firstParentBrain == null || secondParentBrain == null)
            {
                return null;
            }

            GameObject childGo = SpawnBird();

            if (childGo == null)
            {
                return null;
            }

            Brain childBrain = childGo.GetComponent<Brain>();

            if (childBrain == null)
            {
                return null;
            }

            if (!m_EnableMutation || UnityEngine.Random.Range(0f, 1f) > m_MutationProbability)
            {
                childBrain.Init(false);
                childBrain.dna.Combine(firstParentBrain.dna, secondParentBrain.dna);
            }
            else
            {
                childBrain.Init(true);
            }

            return childGo;
        }

        private GameObject SpawnBird()
        {
            if (m_BirdPrefab == null)
            {
                return null;
            }

            float startX = transform.position.x + ((m_RandomlyOffsetSpawnPosition) ? UnityEngine.Random.Range(-m_SpawnPositionRadius, m_SpawnPositionRadius) : 0f);
            float startY = transform.position.y +((m_RandomlyOffsetSpawnPosition) ? UnityEngine.Random.Range(-m_SpawnPositionRadius, m_SpawnPositionRadius) : 0f);
            float startZ = transform.position.z +((m_RandomlyOffsetSpawnPosition) ? UnityEngine.Random.Range(-m_SpawnPositionRadius, m_SpawnPositionRadius) : 0f);

            Vector3 startingPos = new Vector3(startX, startY, startZ);
            Quaternion startingRotation = transform.rotation;

            GameObject birdInstance = Instantiate<GameObject>(m_BirdPrefab);
            birdInstance.transform.position = startingPos;
            birdInstance.transform.rotation = startingRotation;

            return birdInstance;
        }

        private bool AreAllBirdsDead()
        {
            bool allDead = true;

            for (int index = 0; index < m_Population.Count && allDead; ++index)
            {
                GameObject birdGo = m_Population[index];
            
                if (birdGo == null)
                    continue;

                Brain brainComponent = birdGo.GetComponent<Brain>();

                if (brainComponent == null)
                    continue;

                allDead &= !brainComponent.bIsAlive;
            }

            return allDead;
        }

        private void DestroyBirds(List<GameObject> i_Birds)
        {
            if (i_Birds == null)
                return;

            for (int index = 0; index < i_Birds.Count; ++index)
            {
                GameObject bird = i_Birds[index];

                if (bird == null)
                    continue;

                Destroy(bird);
            }

            i_Birds.Clear();
        }

        private void StartNewTrial()
        {
            ++m_GenerationIndex;
            m_ElapsedTime = 0f;

            List<GameObject> sortedPopulation = GetSortedPopulation(m_Population);

            m_Population.Clear();

            for (int index = (int)(3 * sortedPopulation.Count / 4.0f) - 1; index < sortedPopulation.Count - 1; ++index)
            {
                GameObject newBot01 = Breed(sortedPopulation[index], sortedPopulation[index + 1]);
                GameObject newBot02 = Breed(sortedPopulation[index], sortedPopulation[index + 1]);
                GameObject newBot03 = Breed(sortedPopulation[index + 1], sortedPopulation[index]);
                GameObject newBot04 = Breed(sortedPopulation[index + 1], sortedPopulation[index]);

                m_Population.Add(newBot01);
                m_Population.Add(newBot02);
                m_Population.Add(newBot03);
                m_Population.Add(newBot04);
            }

            DestroyBirds(sortedPopulation);
        }

        private List<GameObject> GetSortedPopulation(List<GameObject> i_Population)
        {
            List<GameObject> sorted = new List<GameObject>();

            if (i_Population != null && i_Population.Count > 0)
            {
                sorted.AddRange(i_Population);

                for (int firstIndex = 0; firstIndex < sorted.Count - 1; ++firstIndex)
                {
                    for (int secondIndex = firstIndex + 1; secondIndex < sorted.Count; ++secondIndex)
                    {
                        GameObject first = sorted[firstIndex];
                        GameObject second = sorted[secondIndex];

                        int compare = CompareBirds(first, second);

                        if (compare > 0)
                        {
                            sorted[firstIndex] = second;
                            sorted[secondIndex] = first;
                        }
                    }
                }
            }

            return sorted;
        }

        private int CompareBirds(GameObject i_Main, GameObject i_CompareTo)
        {
            if (i_Main == i_CompareTo || (i_Main == null && i_CompareTo == null))
            {
                return 0;
            }

            if (i_Main == null)
            {
                return -1;
            }

            if (i_CompareTo == null)
            {
                return 1;
            }

            Brain mainBrain = i_Main.GetComponent<Brain>();
            Brain compareToBrain = i_CompareTo.GetComponent<Brain>();

            if (mainBrain == null && compareToBrain == null)
            {
                return 0;
            }

            if (mainBrain == null)
            {
                return -1;
            }

            if (i_CompareTo == null)
            {
                return 1;
            }

            return 0;
        }

        private int GetLivingBotCount()
        {
            int living = 0;

            for (int index = 0; index < m_Population.Count; ++index)
            {
                GameObject birdGo = m_Population[index];

                if (birdGo == null)
                    continue;

                Brain brainComponent = birdGo.GetComponent<Brain>();

                if (brainComponent == null)
                    continue;

                living += (brainComponent.bIsAlive) ? 1 : 0;
            }

            return living;
        }
    }
}