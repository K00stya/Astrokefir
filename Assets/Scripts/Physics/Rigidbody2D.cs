using System;
using UnityEngine;

namespace CustomPhysics
{
    public class Rigidbody2D : MonoBehaviour
    {
        public float ReducingInertia = 0.01f;
        
        [NonSerialized]
        public Vector3 Velocity;

        [NonSerialized]
        public Vector3 AngularVelocity;

        public void AddForce(Vector3 force)
        {
            Velocity += force;
        }
        
        public void AddAngularForce(Vector3 force)
        {
            AngularVelocity += force;
        }
    }
}