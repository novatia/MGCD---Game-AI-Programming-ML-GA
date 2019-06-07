using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace FlappyBird
{
    public class DNA
    {
        // Fields

        private List<int> m_Genes = null;
        private int m_DNALength = 0;
        private int m_MinGeneValue = 0;
        private int m_MaxGeneValue = 0;

        // ACCESSORS

        public int dnaLength
        {
            get
            {
                return m_DNALength;
            }
        }

        // LOGIC

        public void Randomize()
        {
            Internal_Randomize();
        }

        public void SetGene(int i_Index, int i_Gene)
        {
            if (i_Index < 0 || i_Index >= m_Genes.Count)
            {
                return;
            }

            int gene = Mathf.Clamp(i_Gene, m_MinGeneValue, m_MaxGeneValue);
            m_Genes[i_Index] = gene;
        }

        public void Combine(DNA i_FirstDNA, DNA i_SecondDNA)
        {
            m_Genes.Clear();

            int length = Mathf.Min(i_FirstDNA.dnaLength, i_SecondDNA.dnaLength);

            for (int geneIndex = 0; geneIndex < length; ++geneIndex)
            {
                float random01 = UnityEngine.Random.Range(0f, 1f);
                int gene = (random01 < 0.5f) ? i_FirstDNA.GetGene(geneIndex) : i_SecondDNA.GetGene(geneIndex);
                m_Genes.Add(gene);
            }
        }

        public void Mutate()
        {
            int randomIndex = UnityEngine.Random.Range(0, m_Genes.Count);
            int randomGene = UnityEngine.Random.Range(m_MinGeneValue, m_MaxGeneValue);

            SetGene(randomIndex, randomGene);
        }

        public int GetGene(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Genes.Count)
            {
                return 0;
            }

            return m_Genes[i_Index];
        }

        // INTERNALS

        private void Internal_Randomize()
        {
            m_Genes.Clear();

            for (int geneIndex = 0; geneIndex < m_DNALength; ++geneIndex)
            {
                int geneValue = UnityEngine.Random.Range(m_MinGeneValue, m_MaxGeneValue);
                m_Genes.Add(geneValue);
            }
        }

        // CTOR

        public DNA(int i_DNALength, int i_MinGeneValue, int i_MaxGeneValue, bool i_Randomize = true)
        {
            m_Genes = new List<int>();

            m_DNALength = Mathf.Max(1, i_DNALength);
            m_MinGeneValue = i_MinGeneValue;
            m_MaxGeneValue = Mathf.Max(m_MinGeneValue, i_MaxGeneValue);

            if (i_Randomize)
            {
                Randomize();
            }
        }
    }
}