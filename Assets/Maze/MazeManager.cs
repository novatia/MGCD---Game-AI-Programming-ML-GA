using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class MazeManager : MonoBehaviour
    {
        // Serializable fields.

        [SerializeField]
        private int m_MazeWidth = 40;
        [SerializeField]
        private int m_MazeDepth = 40;
        [SerializeField]
        private Vector3 m_MazeStartPosition = Vector3.zero;
        [SerializeField]
        private int m_MazeWidthSafeZone = 3;
        [SerializeField]
        private int m_MazeDepthSafeZone = 3;

        [SerializeField]
        private PopulationManager m_PopulationManagerPrefab = null;

        // Fields

        private MazeGenerator m_MazeGenerator = null;
        private PopulationManager m_PopulationManagerInstance = null;

        // MonoBehaviour's interface

        private void Awake()
        {
            m_MazeGenerator = GetComponent<MazeGenerator>();
        }

        private void Start()
        {
            if (m_MazeGenerator != null)
            {
                m_MazeGenerator.GenerateMaze(m_MazeWidth, m_MazeDepth, m_MazeStartPosition, m_MazeWidthSafeZone, m_MazeDepthSafeZone);
            }
        }

        private void Update()
        {
            if (m_PopulationManagerInstance == null)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Internal_SpawnPopulationManager();
                }
            }
        }

        private void OnDestroy()
        {
            Internal_DestroyPopulationManager();
        }

        // INTERNALS

        private void Internal_SpawnPopulationManager()
        {
            if (m_PopulationManagerPrefab == null)
            {
                return;
            }

            m_PopulationManagerInstance = Instantiate<PopulationManager>(m_PopulationManagerPrefab);
        }

        private void Internal_DestroyPopulationManager()
        {
            if (m_PopulationManagerInstance == null)
            {
                return;
            }

            Destroy(m_PopulationManagerInstance.gameObject);
            m_PopulationManagerInstance = null;
        }
    }
}