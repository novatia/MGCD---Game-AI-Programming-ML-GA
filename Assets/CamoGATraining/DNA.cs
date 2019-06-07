using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace CamoGATraining
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class DNA : MonoBehaviour
    {
        // Serializable fields

        [SerializeField]
        private float m_MinScaleMultiplier = 0.3f;
        [SerializeField]
        private float m_MaxScaleMultiplier = 1f;

        // Fields

        private float m_Red = 0f;
        private float m_Green = 0f;
        private float m_Blue = 0f;

        private float m_Size = 1f;
        private float m_BaseScale = 1f;

        private float m_LiveTimer = 0f;
        private bool m_Dead = false;

        private Collider2D m_Collider2D = null;
        private SpriteRenderer m_SpriteRenderer = null;

        // ACCESSORS

        public float red
        {
            get { return m_Red; }
            set { m_Red = value; Internal_UpdateColor(); }
        }

        public float green
        {
            get { return m_Green; }
            set { m_Green = value; Internal_UpdateColor(); }
        }

        public float blue
        {
            get { return m_Blue; }
            set { m_Blue = value; Internal_UpdateColor(); }
        }

        public float liveTimer
        {
            get { return m_LiveTimer; }
        }

        public bool dead
        {
            get { return m_Dead; }
        }

        public float size
        {
            get
            {
                return m_Size;
            }
            set
            {
                m_Size = value;
                Internal_UpdateSize();
            }
        }

        // MonoBehaviour's interface

        private void OnMouseDown()
        {
            Internal_Die();
        }

        private void Awake()
        {
            m_Collider2D = GetComponent<Collider2D>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();

            m_BaseScale = transform.localScale.x;
        }

        private void Start()
        {
            m_LiveTimer = PeopleManager.trialDurationMain;
        }

        // INTERNALS

        private void Internal_Show()
        {
            m_Collider2D.enabled = true;
            m_SpriteRenderer.enabled = false;
        }

        private void Internal_Hide()
        {
            m_Collider2D.enabled = false;
            m_SpriteRenderer.enabled = false;
        }

        private void Internal_UpdateColor()
        {
            m_SpriteRenderer.color = new Color(red, green, blue);
        }

        private void Internal_Die()
        {
            if (m_Dead)
                return;

            Internal_Hide();

            m_LiveTimer = PeopleManager.elapsedTimeMain;

            m_Dead = true;
        }

        private void Internal_Reborn()
        {
            if (!m_Dead)
                return;

            Internal_Show();

            m_LiveTimer = PeopleManager.trialDurationMain;

            m_Dead = false;
        }

        private void Internal_UpdateSize()
        {
            float targetScaleMultiplier = Mathf.Lerp(m_MinScaleMultiplier, m_MaxScaleMultiplier, Mathf.Clamp01(m_Size));
            Internal_SetScale(m_BaseScale * targetScaleMultiplier);
        }

        private void Internal_SetScale(float i_Scale)
        {
            transform.localScale = new Vector3(i_Scale, i_Scale, i_Scale);
        }
    }
}