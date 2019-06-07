using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace SingleGeneV2
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class Brain : MonoBehaviour
    {
        private enum DNAGene
        {
            Forward = 0,
            Backward = 1,
            Left = 2,
            Right = 3,
            Jump = 4,
            Crouch = 5,
        }

        // Serializable fields

        [SerializeField]
        private int m_DNALength = 1;
        [SerializeField]
        private int m_MaxGeneValue = 0;

        // Fields

        private Vector3 m_LastPosition = Vector3.zero;

        private float m_DistanceRun = 0f;
        private float m_LiveTimer = 0f;
        private DNA m_DNA;

        private ThirdPersonCharacter m_Character;
        private bool m_IsAlive = false;

        // ACCESSORS

        public float distanceRun
        {
            get
            {
                return m_DistanceRun;
            }
        }

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
            m_Character = GetComponent<ThirdPersonCharacter>();
        }

        private void Start()
        {
            m_LastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (!m_IsAlive)
            {
                m_Character.Move(Vector3.zero, false, false);
                return;
            }

            // Update distance run.

            Internal_UpdateDistanceRun();

            // Get Action from DNA.

            float horizontal = 0f;
            float vertical = 0f;

            bool crouch = false;
            bool jump = false;

            DNAGene firstGene = (DNAGene)m_DNA.GetGene(0);

            if (firstGene == DNAGene.Forward) vertical = 1f;
            else if (firstGene == DNAGene.Backward) vertical = -1f;
            else if (firstGene == DNAGene.Left) horizontal = -1f;
            else if (firstGene == DNAGene.Right) horizontal = 1f;
            else if (firstGene == DNAGene.Jump) jump = true;
            else if (firstGene == DNAGene.Crouch) crouch = true;

            Vector3 move = vertical * Vector3.forward + horizontal * Vector3.right;
            m_Character.Move(move, crouch, jump);

            m_LiveTimer += Time.fixedDeltaTime;
        }

        private void OnCollisionEnter(Collision i_Collision)
        {
            if (i_Collision.gameObject.tag == "dead")
            {
                Internal_Die();
            }
        }

        // LOGIC

        public void Init(bool i_RandomizeDNA = true)
        {
            m_DNA = new DNA(m_DNALength, m_MaxGeneValue, i_RandomizeDNA);

            m_IsAlive = true;
            m_LiveTimer = 0f;
            m_DistanceRun = 0f;
        }

        // INTERNALS

        private void Internal_Die()
        {
            if (!m_IsAlive)
                return;

            m_IsAlive = false;
        }

        private void Internal_UpdateDistanceRun()
        {
            Vector3 currentPosition = transform.position;

            Vector3 offset = (currentPosition - m_LastPosition);
            offset.y = 0f;

            float offsetSize = offset.magnitude;

            m_DistanceRun += offsetSize;

            m_LastPosition = currentPosition;
        }
    }
}