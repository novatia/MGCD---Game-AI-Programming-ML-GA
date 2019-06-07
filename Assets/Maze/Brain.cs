using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace Maze
{
    public class Brain : MonoBehaviour
    {
        private enum DNAGene
        {
            Rotate = 1,
        }

        // Serializable fields

        [SerializeField]
        private int m_DNALength = 2;
        [SerializeField]
        private int m_MaxGeneValue = 3;

        [SerializeField]
        private Eyes m_Eyes = null;
        [SerializeField]
        private string m_WallTag = "wall";

        [SerializeField]
        bool m_DieTouchingWall = false;
        [SerializeField]
        private bool m_ResetDataOnDead = false;

        // Fields

        private Rigidbody m_Body = null;

        private GameObject m_CharacterInstance = null;

        private float m_LiveTimer = 0f;
        private DNA m_DNA;

        private bool m_IsAlive = false;

        private bool m_SeeWall = false;

        // ACCESSORS

        public float liveTimer
        {
            get { return m_LiveTimer; }
        }

        public DNA dna
        {
            get { return m_DNA; }
        }

        public bool isAlive
        {
            get
            {
                return m_IsAlive;
            }
        }

        // MonoBehaviour's interface

        private void Awake()
        {
            m_Body = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!m_IsAlive)
            {
                return;
            }
        }

        private void FixedUpdate()
        {
            if (!m_IsAlive)
            {
                return;
            }

            // Update See Ground.

            m_SeeWall = (m_Eyes != null) ? m_Eyes.CanSeeTag(m_WallTag) : false;

            // Get Action from DNA.

            m_LiveTimer += Time.fixedDeltaTime;
        }

        private void OnDestroy()
        {
            if (m_CharacterInstance != null)
            {
                Destroy(m_CharacterInstance);
                m_CharacterInstance = null;
            }
        }

        private void OnCollisionEnter(Collision i_Collision)
        {
            if (i_Collision.gameObject.tag == "dead")
            {
                Internal_Die();
            }
            else
            {
                if (m_DieTouchingWall && i_Collision.gameObject.tag == "wall")
                {
                    Internal_Die();
                }
            }
        }

        private void OnTriggerEnter(Collider i_Collider)
        {
            if (i_Collider.gameObject.tag == "dead")
            {
                Internal_Die();
            }
            else
            {
                if (m_DieTouchingWall && i_Collider.gameObject.tag == "wall")
                {
                    Internal_Die();
                }
            }
        }

        // LOGIC

        public void Init(bool i_RandomizeDNA = true)
        {
            m_DNA = new DNA(m_DNALength, m_MaxGeneValue, i_RandomizeDNA);

            m_IsAlive = true;
            m_LiveTimer = 0f;
        }

        // INTERNALS

        private void Internal_Die()
        {
            if (!m_IsAlive)
                return;

            if (m_Body != null)
            {
                m_Body.velocity = Vector3.zero;
                m_Body.angularVelocity = Vector3.zero;
            }

            if (m_ResetDataOnDead)
            {
                m_LiveTimer = 0f;
            }

            m_IsAlive = false;
        }
    }
}