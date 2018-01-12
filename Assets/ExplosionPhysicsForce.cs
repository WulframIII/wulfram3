using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Wulfram3;

namespace UnityStandardAssets.Effects
{
    public class ExplosionPhysicsForce : Photon.PunBehaviour {
        public float explosionForce = 4;

        private void Start()
        {
            float multiplier = GetComponent<ParticleSystemMultiplier>().multiplier;
            float r = 10*multiplier;
            Collider[] cols = Physics.OverlapSphere(transform.position, r);
            foreach (Collider col in cols)
            {
                if (col.attachedRigidbody != null)
                {
                    col.attachedRigidbody.AddExplosionForce(explosionForce * multiplier, transform.position, r, 1 * multiplier, ForceMode.Impulse);
                }
            }
        }

    }
}
