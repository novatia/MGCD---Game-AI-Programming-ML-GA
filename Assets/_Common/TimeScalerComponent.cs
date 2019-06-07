using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public class TimeScalerComponent : MonoBehaviour
{
    // Serializable fields

    [Header("Faster")]

    [SerializeField]
    private KeyCode m_IncrementKey = KeyCode.M;
    [SerializeField]
    private float m_IncrementAmount = 0.1f;

    [Header("Slower")]

    [SerializeField]
    private KeyCode m_ReduceKey = KeyCode.N;
    [SerializeField]
    private float m_ReduceAmount = 0.1f;

    // Fields

    private float m_OriginalTimeScaleMultiplier = 1f;
    private float m_CurrentTimeScaleMultiplier = 1f;

    // ACCESSORS

    public float currentTimeScaleMultiplier
    {
        get { return m_CurrentTimeScaleMultiplier; }
    }

    // MonoBehaviour's interface

    private void Start()
    {
        m_OriginalTimeScaleMultiplier = Time.timeScale;
        m_CurrentTimeScaleMultiplier = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(m_IncrementKey))
        {
            m_CurrentTimeScaleMultiplier += m_IncrementAmount;
        }

        if (Input.GetKeyDown(m_ReduceKey))
        {
            m_CurrentTimeScaleMultiplier -= m_ReduceAmount;
        }

        m_CurrentTimeScaleMultiplier = Mathf.Max(0f, m_CurrentTimeScaleMultiplier);

        float scale = m_OriginalTimeScaleMultiplier * m_CurrentTimeScaleMultiplier;
        Time.timeScale = scale;
    }
}
