using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class SkidTrail : MonoBehaviour
    {
#pragma warning disable 649
        [SerializeField] private float m_PersistTime;
#pragma warning restore 649


        private IEnumerator Start()
        {
			while (true)
            {
                yield return null;

                if (transform.parent.parent == null)
                {
					Destroy(gameObject, m_PersistTime);
                }
            }
        }
    }
}
