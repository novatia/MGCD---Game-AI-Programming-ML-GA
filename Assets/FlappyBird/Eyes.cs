using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace FlappyBird
{
    public class Eyes : MonoBehaviour
    {
        // Serializable fields

        [SerializeField]
        private float m_DefaultRaycastLength = 10f;
        [SerializeField]
        private LayerMask m_DeafultRaycastLayerMask = 0;

        // LOGIC

        public bool CanSeeTag(string i_Tag, Vector2 i_Direction)
        {
            return CanSeeTag(i_Tag, i_Direction, m_DefaultRaycastLength);
        }

        public bool CanSeeTag(string i_Tag, Vector2 i_Direction, float i_MaxDistance)
        {
            return CanSeeTag(i_Tag, i_Direction, i_MaxDistance, m_DeafultRaycastLayerMask);
        }

        public bool CanSeeTag(string i_Tag, Vector2 i_Direction, float i_MaxDistance, LayerMask i_LayerMask)
        {
            RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, i_Direction, i_MaxDistance, i_LayerMask);

            if (raycastHit)
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