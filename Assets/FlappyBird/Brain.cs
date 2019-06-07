using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace FlappyBird
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Brain : MonoBehaviour
    {
        // Serializable fields

        [SerializeField]
        private int m_DNALength = 5;
        [SerializeField]
        private int m_MinGeneValue = 0;
        [SerializeField]
        private int m_MaxGeneValue = 0;

        [SerializeField]
        private Eyes m_Eyes = null;

        [SerializeField]
        private float m_EyesSightDistance = 1f;

        [SerializeField]
        private float m_fSpeedFactor = 0.1f;

        [SerializeField]
        private bool m_ResetDataOnDeath = false;

        // Fields

        private DNA m_DNA;

        private bool m_bSeeDownWall = false;
        private bool m_bSeeUpWall = false;
        private bool m_bSeeBottom = false;
        private bool m_bSeeTop = false;

        private float m_LiveTimer = 0f;
        private bool m_bIsAlive = false;

        private Rigidbody2D m_Body2d = null;

        private bool m_Init = false;

        // ACCESSORS

        public DNA dna
        {
            get
            {
                return m_DNA;
            }
        }

        public float liveTimer
        {
            get
            {
                return m_LiveTimer;
            }
        }

        public bool bIsAlive
        {
            get
            {
                return m_bIsAlive;
            }
        }

        // MonoBehaviour's interface

        private void Awake()
        {
            m_Body2d = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D i_Collision)
        {
            if (!m_Init)
            {
                return;
            }

            bool bTouchDeadTag = (i_Collision.gameObject.tag == "dead");
            bool bTouchTopTag = (i_Collision.gameObject.tag == "top");
            bool bTouchBottomTag = (i_Collision.gameObject.tag == "bottom");
            bool bTouchUpWallTag = (i_Collision.gameObject.tag == "upwall");
            bool bTouchDownWallTag = (i_Collision.gameObject.tag == "downwall");

            if (m_bIsAlive)
            {
                if (bTouchDeadTag)
                {
                    m_bIsAlive = false;

                    if (m_ResetDataOnDeath)
                    {
                        m_LiveTimer = 0f;
                    }
                }
            }
        }

        private void OnCollisionStay2D(Collision2D i_Collision)
        {
            if (!m_Init)
            {
                return;
            }

            bool bTouchTopTag = (i_Collision.gameObject.tag == "top");
            bool bTouchBottomTag = (i_Collision.gameObject.tag == "bottom");
            bool bTouchUpWallTag = (i_Collision.gameObject.tag == "upwall");
            bool bTouchDownWallTag = (i_Collision.gameObject.tag == "downwall");

            if (m_bIsAlive)
            {

            }
        }

        private void Update()
        {
            if (!m_bIsAlive || !m_Init)
            {
                return;
            }

            m_bSeeUpWall = (m_Eyes != null) ? m_Eyes.CanSeeTag("upwall", new Vector2(1,1), m_EyesSightDistance) : false;
            m_bSeeDownWall = (m_Eyes != null) ? m_Eyes.CanSeeTag("downwall", new Vector2(1, -1), m_EyesSightDistance) : false;
            m_bSeeTop = (m_Eyes != null) ? m_Eyes.CanSeeTag("top", Vector2.up, m_EyesSightDistance) : false;
            m_bSeeBottom = (m_Eyes != null) ? m_Eyes.CanSeeTag("bottom", Vector2.down, m_EyesSightDistance) : false;

            m_LiveTimer += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = Vector2.zero;
            if (!m_bIsAlive || !m_Init)
            {
                return;
            }

            if (!m_bSeeBottom && !m_bSeeDownWall)
            {
                velocity.x = (1 * Math.Abs(m_DNA.GetGene(0)) * m_fSpeedFactor);
            }

            if (!m_bSeeDownWall)
            {
                velocity.y = -(1 * Math.Abs(m_DNA.GetGene(1)) * m_fSpeedFactor);
            }
            else
            {
                velocity.y = (1 * Math.Abs(m_DNA.GetGene(0)) * m_fSpeedFactor);
            }



            m_Body2d.velocity = velocity;

        }

        // LOGIC

        public void Init(bool i_RandomizeDNA)
        {
            if (m_Init)
                return;

            m_DNA = new DNA(m_DNALength, m_MinGeneValue, m_MaxGeneValue, i_RandomizeDNA);

            m_bIsAlive = true;
            m_LiveTimer = 0f;

            m_Init = true;
        }
    }
}