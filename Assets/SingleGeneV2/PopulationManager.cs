using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SingleGeneV2
{
    public class PopulationManager : MonoBehaviour
    {
        // Serializable fields

        [SerializeField]
        private GameObject m_CharacterPrefab = null;
        [SerializeField]
        private int m_PopulationSize = 50;
        [SerializeField]
        private float m_TrialTime = 5;
        [SerializeField]
        private bool m_EnableMutation = false;
        [SerializeField]
        private float m_MutationProbability = 0f;

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
            GUI.Label(new Rect(10, 100, 200, 30), "Living: " + GetLivingCharacterCount(), guiStyle);
            GUI.EndGroup();
        }

        private void Start()
        {
            for (int index = 0; index < m_PopulationSize; ++index)
            {
                GameObject characterInstance = SpawnCharacter();

                Brain characterBrain = characterInstance.GetComponent<Brain>();

                if (characterBrain == null)
                {
                    Destroy(characterInstance);
                    continue;
                }

                characterBrain.Init();
                m_Population.Add(characterInstance);
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
                if (AreAllCharactersDead() || Input.GetKeyDown(KeyCode.N))
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

            GameObject childGo = SpawnCharacter();

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

        private GameObject SpawnCharacter()
        {
            if (m_CharacterPrefab == null)
            {
                return null;
            }

            float startX = transform.position.x;// + UnityEngine.Random.Range(-2, 2);
            float startY = transform.position.y;// + UnityEngine.Random.Range(-2, 2);
            float startZ = transform.position.z;// + UnityEngine.Random.Range(-2, 2);

            Vector3 startingPos = new Vector3(startX, startY, startZ);
            Quaternion startingRotation = transform.rotation;

            GameObject characterInstance = Instantiate<GameObject>(m_CharacterPrefab);
            characterInstance.transform.position = startingPos;
            characterInstance.transform.rotation = startingRotation;

            return characterInstance;
        }

        private bool AreAllCharactersDead()
        {
            bool allDead = true;

            for (int index = 0; index < m_Population.Count && allDead; ++index)
            {
                GameObject characterGo = m_Population[index];

                if (characterGo == null)
                    continue;

                Brain brainComponent = characterGo.GetComponent<Brain>();

                if (brainComponent == null)
                    continue;

                allDead &= !brainComponent.isAlive;
            }

            return allDead;
        }

        private void DestroyCharacters(List<GameObject> i_Characters)
        {
            if (i_Characters == null)
                return;

            for (int index = 0; index < i_Characters.Count; ++index)
            {
                GameObject character = i_Characters[index];

                if (character == null)
                    continue;

                Destroy(character);
            }

            i_Characters.Clear();
        }

        private void StartNewTrial()
        {
            ++m_GenerationIndex;
            m_ElapsedTime = 0f;

            List<GameObject> sortedPopulation = GetSortedPopulation(m_Population);

            m_Population.Clear();

            for (int index = (int)(sortedPopulation.Count / 2f) - 1; index < sortedPopulation.Count - 1; ++index)
            {
                GameObject newCharacter01 = Breed(sortedPopulation[index], sortedPopulation[index + 1]);
                GameObject newCharacter02 = Breed(sortedPopulation[index + 1], sortedPopulation[index]);

                m_Population.Add(newCharacter01);
                m_Population.Add(newCharacter02);
            }

            DestroyCharacters(sortedPopulation);
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

                        int compare = CompareCharacters(first, second);

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

        private int CompareCharacters(GameObject i_Main, GameObject i_CompareTo)
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

            if (mainBrain.isAlive == compareToBrain.isAlive)
            {
                if (mainBrain.liveTimer > compareToBrain.liveTimer)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (mainBrain.isAlive)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        private int GetLivingCharacterCount()
        {
            int living = 0;

            for (int index = 0; index < m_Population.Count; ++index)
            {
                GameObject characterGo = m_Population[index];

                if (characterGo == null)
                    continue;

                Brain brainComponent = characterGo.GetComponent<Brain>();

                if (brainComponent == null)
                    continue;

                living += (brainComponent.isAlive) ? 1 : 0;
            }

            return living;
        }
    }
}