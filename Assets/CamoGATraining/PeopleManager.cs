using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CamoGATraining
{
    public class PeopleManager : MonoBehaviour
    {
        // STATIC - Private

        private static PeopleManager s_Instance = null;

        // STATIC - Public

        public static PeopleManager instance
        {
            get
            {
                return s_Instance;
            }
        }

        public static float elapsedTimeMain
        {
            get
            {
                if (s_Instance)
                {
                    return s_Instance.elapsedTime;
                }

                return 0f;
            }
        }

        public static float trialDurationMain
        {
            get
            {
                if (s_Instance)
                {
                    return s_Instance.trialDuration;
                }

                return 0f;
            }
        }

        // Serializable fields.

        [SerializeField]
        private GameObject m_PersonPrefab = null;
        [SerializeField]
        private int m_PeopleCount = 10;
        [SerializeField]
        private float m_TrialDuration = 10f;

        [SerializeField]
        private bool m_EnableMutation = false;
        [SerializeField]
        private float m_MutationProbability = 0f;

        // Fields

        private List<GameObject> m_Peoples = new List<GameObject>();

        private int m_GenerationIndex = 0;
        private float m_ElapsedTime = 0f;

        // ACCESSORS

        public float elapsedTime
        {
            get { return m_ElapsedTime; }
        }

        public float trialDuration
        {
            get
            {
                return m_TrialDuration;
            }
        }

        // MonoBehaviour's interface

        private void OnGUI()
        {
            GUIStyle guiStyle = new GUIStyle();
            guiStyle.fontSize = 50;
            guiStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + (m_GenerationIndex + 1), guiStyle);
            GUI.Label(new Rect(10, 65, 100, 20), "Trial time: " + m_ElapsedTime.ToString("F2"), guiStyle);
        }

        private void Awake()
        {
            if (s_Instance) Destroy(s_Instance.gameObject);
            s_Instance = this;
        }

        private void Start()
        {
            SpawnPeople(m_PeopleCount);
        }

        private void Update()
        {
            m_ElapsedTime += Time.deltaTime;

            if (m_ElapsedTime > m_TrialDuration)
            {
                StartNewTrial();
            }
            else
            {
                if (AreAllPeopleDead() || Input.GetKeyDown(KeyCode.N))
                {
                    StartNewTrial();
                }
            }
        }

        private void OnDestroy()
        {
            if (s_Instance == this) s_Instance = null;
        }

        // INTERNALS

        private void SpawnPeople(int i_Count)
        {
            m_Peoples.Clear();

            for (int index = 0; index < i_Count; ++index)
            {
                GameObject personGo = SpawnNewPerson();

                if (personGo == null)
                    continue;

                float dnaRed = UnityEngine.Random.Range(0f, 1f);
                float dnaGreen = UnityEngine.Random.Range(0f, 1f);
                float dnaBlue = UnityEngine.Random.Range(0f, 1f);
                float dnaSize = UnityEngine.Random.Range(0f, 1f);

                DNA dnaComponent = personGo.GetComponent<DNA>();
                if (dnaComponent != null)
                {
                    dnaComponent.red = dnaRed;
                    dnaComponent.green = dnaGreen;
                    dnaComponent.blue = dnaBlue;
                    dnaComponent.size = dnaSize;
                }

                UpdatePersonName(personGo);

                m_Peoples.Add(personGo);
            }
        }

        private void UpdatePersonName(GameObject i_Person)
        {
            if (i_Person == null)
                return;

            DNA dnaComponent = i_Person.GetComponent<DNA>();

            if (dnaComponent != null)
            {
                i_Person.name = "Person (" + dnaComponent.red.ToString("F2") + ", " + dnaComponent.green.ToString("F2") + ", " + dnaComponent.blue.ToString("F2") + ")";
            }
            else
            {
                i_Person.name = "Persona (No DNA)";
            }
        }

        private bool AreAllPeopleDead()
        {
            bool allDead = true;

            for (int index = 0; index < m_Peoples.Count && allDead; ++index)
            {
                GameObject peopleGo = m_Peoples[index];

                if (peopleGo == null)
                    continue;

                DNA dnaComponent = peopleGo.GetComponent<DNA>();

                if (dnaComponent == null)
                    continue;

                allDead &= dnaComponent.dead;
            }

            return allDead;
        }

        private void StartNewTrial()
        {
            m_ElapsedTime = 0f;
            ++m_GenerationIndex;

            List<GameObject> newPopulation = new List<GameObject>();
            List<GameObject> sortedPopulation = m_Peoples.OrderBy(o => o.GetComponent<DNA>().liveTimer).ToList();

            m_Peoples.Clear();

            for (int index = (int)(sortedPopulation.Count / 2f) - 1; index < sortedPopulation.Count - 1; ++index)
            {
                GameObject newPerson01 = Breed(sortedPopulation[index], sortedPopulation[index + 1]);
                GameObject newPerson02 = Breed(sortedPopulation[index + 1], sortedPopulation[index]);

                if (newPerson01 != null) m_Peoples.Add(newPerson01);
                if (newPerson02 != null) m_Peoples.Add(newPerson02);
            }

            DestroyPeople(sortedPopulation);
        }

        private GameObject Breed(GameObject i_FirstParent, GameObject i_SecondParent)
        {
            if (i_FirstParent == null || i_SecondParent == null)
            {
                return null;
            }

            DNA firstDna = i_FirstParent.GetComponent<DNA>();
            DNA secondDna = i_SecondParent.GetComponent<DNA>();

            if (firstDna == null || secondDna == null)
            {
                return null;
            }

            GameObject newPerson = SpawnNewPerson();

            DNA newPersonDna = newPerson.GetComponent<DNA>();

            if (newPersonDna == null)
            {
                Destroy(newPerson);
                return null;
            }

            if (!m_EnableMutation || UnityEngine.Random.Range(0f, 1f) > m_MutationProbability)
            {
                newPersonDna.red = (UnityEngine.Random.Range(0f, 1f) < 0.5f) ? firstDna.red : secondDna.red;
                newPersonDna.green = (UnityEngine.Random.Range(0f, 1f) < 0.5f) ? firstDna.green : secondDna.green;
                newPersonDna.blue = (UnityEngine.Random.Range(0f, 1f) < 0.5f) ? firstDna.blue : secondDna.blue;
                newPersonDna.size = (UnityEngine.Random.Range(0f, 1f) < 0.5f) ? firstDna.size : secondDna.size;
            }
            else
            {
                newPersonDna.red = UnityEngine.Random.Range(0f, 1f);
                newPersonDna.green = UnityEngine.Random.Range(0f, 1f);
                newPersonDna.blue = UnityEngine.Random.Range(0f, 1f);
                newPersonDna.size = UnityEngine.Random.Range(0f, 1f);
            }

            UpdatePersonName(newPerson);

            return newPerson;
        }

        private void DestroyPeople(List<GameObject> i_People)
        {
            if (i_People == null)
                return;

            for (int index = 0; index < i_People.Count; ++index)
            {
                GameObject person = i_People[index];

                if (person == null)
                    continue;

                Destroy(person);
            }

            i_People.Clear();
        }

        private GameObject SpawnNewPerson()
        {
            if (m_PersonPrefab == null)
            {
                return null;
            }

            Vector3 pos = new Vector3(UnityEngine.Random.Range(-9f, 9f), UnityEngine.Random.Range(-4.5f, 4.5f), 0f);
            GameObject personGo = Instantiate<GameObject>(m_PersonPrefab);
            personGo.transform.position = pos;
            personGo.transform.rotation = Quaternion.identity;

            return personGo;
        }
    }
}