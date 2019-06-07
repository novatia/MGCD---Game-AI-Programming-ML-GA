using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace Maze
{
    public class Eyes : MonoBehaviour
    {
        // Serializable fields

        [SerializeField]
        private float m_RaycastLength = 10f;
        [SerializeField]
        private LayerMask m_RaycastLayerMask = 0;
        [SerializeField]
        private bool m_DrawGizmos = true;

        // MonoBehaviour's interface

        private void OnDrawGizmos()
        {
            if (!m_DrawGizmos)
                return;

            Vector3 start = transform.position;
            Vector3 end = start + transform.forward * m_RaycastLength;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(start, end);
        }

        // LOGIC

        public bool CanSeeTag(string i_Tag)
        {
            RaycastHit raycastHit;
            bool raycastSuccess = Physics.Raycast(transform.position, transform.forward, out raycastHit, m_RaycastLength, m_RaycastLayerMask);

            if (raycastSuccess)
            {
                if (raycastHit.collider.gameObject.tag == i_Tag)
                {
                    return true;
                }
            }

            return false;
        }
    }
}