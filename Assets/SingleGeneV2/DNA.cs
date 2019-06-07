using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SingleGeneV2
{
    public class DNA
    {
        // Fields

        private int m_DNALength = 0;
        private int m_MaxGeneValue = 0;

        private List<int> m_Genes = null;

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
            Internal_RegenerateRandom();   
        }

        public void SetGene(int i_Index, int i_Gene)
        {
            if (i_Index < 0 || i_Index >= m_Genes.Count)
            {
                return;
            }

            m_Genes[i_Index] = i_Gene;
        }

        public void Combine(DNA i_FirstDNA, DNA i_SecondDNA)
        {
            if (i_FirstDNA == null || i_SecondDNA == null)
            {
                return;
            }

            for (int index = 0; index < m_DNALength; ++index)
            {
                bool firstHalf = (index < m_DNALength / 2);

                DNA parentDNA = (firstHalf) ? i_FirstDNA : i_SecondDNA;

                int parentGene = parentDNA.GetGene(index);
                SetGene(index, parentGene);
            }
        }

        public void Mutate()
        {
            int randomIndex = UnityEngine.Random.Range(0, m_Genes.Count);
            int randomGene = UnityEngine.Random.Range(0, m_MaxGeneValue);

            SetGene(randomIndex, randomGene);
        }

        public int GetGene(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Genes.Count)
            {
                return -1;
            }

            return m_Genes[i_Index];
        }

        // INTERNALS

        private void Internal_RegenerateRandom()
        {
            m_Genes.Clear();

            for (int index = 0; index < m_DNALength; ++index)
            {
                int randomGene = UnityEngine.Random.Range(0, m_MaxGeneValue);
                m_Genes.Add(randomGene);
            }
        }

        // CTOR

        public DNA(int i_DNALength, int i_MaxGeneValue, bool i_Randomize = true)
        {
            m_Genes = new List<int>();

            m_DNALength = Mathf.Max(1, i_DNALength);
            m_MaxGeneValue = Mathf.Max(0, i_MaxGeneValue);

            Randomize();
        }
    }
}