using System.Collections;
using UnityEngine;

using System.Collections.Generic;

namespace Maze
{
    public class MazeGenerator : MonoBehaviour
    {
        // Serializable fields

        [SerializeField]
        private GameObject m_BlockPrefab = null;

        // Fields

        private List<GameObject> m_Walls = new List<GameObject>();

        // MonoBehaviour's interface

        private void OnDestroy()
        {
            for (int index = 0; index < m_Walls.Count; ++index)
            {
                GameObject wallGo = m_Walls[index];

                if (wallGo == null)
                    continue;

                Destroy(wallGo);
            }

            m_Walls.Clear();
        }

        // LOGIC

        public void GenerateMaze(int i_Width, int i_Depth, Vector3 i_StartPosition, int i_SafeZoneWidth = 3, int i_SafeZoneDepth = 3)
        {
            for (int currentWidth = 0; currentWidth < i_Width; ++currentWidth)
            {
                for (int currentDepth = 0; currentDepth < i_Depth; currentDepth++)
                {
                    GameObject wallGo = null;

                    if (currentWidth == 0 || currentDepth == 0)
                    {
                        wallGo = Instantiate<GameObject>(m_BlockPrefab, new Vector3(currentWidth + i_StartPosition.x, i_StartPosition.y, currentDepth + i_StartPosition.z), Quaternion.identity);
                    }
                    else if (currentWidth < i_SafeZoneWidth && currentDepth < i_SafeZoneDepth)
                    {
                        continue;
                    }
                    else if (currentWidth == i_Width - 1 || currentDepth == i_Depth - 1)
                    {
                        wallGo = Instantiate<GameObject>(m_BlockPrefab, new Vector3(currentWidth + i_StartPosition.x, i_StartPosition.y, currentDepth + i_StartPosition.z), Quaternion.identity);
                    }
                    else if (Random.Range(0f, 1f) < 0.2f)
                    {
                        wallGo = Instantiate<GameObject>(m_BlockPrefab, new Vector3(currentWidth + i_StartPosition.x, i_StartPosition.y, currentDepth + i_StartPosition.z), Quaternion.identity);
                    }

                    if (wallGo != null)
                    {
                        wallGo.name = "Wall [" + currentWidth + ", " + currentDepth + "]";
                        wallGo.transform.SetParent(transform, true);

                        m_Walls.Add(wallGo);
                    }
                }
            }
        }
    }
}