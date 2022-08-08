using System.Collections.Generic;
using UnityEngine;

namespace CustomPhysics
{
    public class PhysicsSimulation
    {
        /*
         * All types of colliders should be checked,
         * but there is enough circle on the test task
         */
        public static List<CircleCollider2D> Colliders = new List<CircleCollider2D>(); 
        public static List<Rigidbody2D> Rigidbodies = new List<Rigidbody2D>(); 

        public void FixedUpdate(float fixedDeltaTime)
        {
            foreach (var rb in Rigidbodies)
            {
                if(!rb.gameObject.activeSelf) continue;
                
                //movement
                rb.transform.position += rb.Velocity * fixedDeltaTime;
                rb.Velocity -= rb.Velocity * rb.ReducingInertia;
                if (rb.Velocity.magnitude <= 0.01f)
                {
                    rb.Velocity = Vector3.zero;
                }
                
                //rotation
                rb.transform.rotation *= Quaternion.Euler(
                    rb.AngularVelocity * fixedDeltaTime);
                rb.AngularVelocity -= rb.AngularVelocity * rb.ReducingInertia;
                if (rb.AngularVelocity.magnitude <= 0.01f)
                {
                    rb.AngularVelocity = Vector3.zero;
                }
            }
            
            //collisions
            for (int i = 0; i < Colliders.Count; i++)
            {
                for (int j = i + 1; j < Colliders.Count; j++)
                {
                    if(!Colliders[i].gameObject.activeSelf || !Colliders[j].gameObject.activeSelf) continue;
                    
                    var a = Colliders[i];
                    var b = Colliders[j];
                    
                    var distance = Vector3.Distance(a.transform.position, b.transform.position);
                    if (distance <= a.Radius + b.Radius)
                    {
                        a.OnCollision?.Invoke(b);
                        b.OnCollision?.Invoke(a);
                    }
                }
            }
        }

        public static List<Collider2D> RayCast2D(Ray ray, float lenght = 100f)
        {
            List<Collider2D> colliders = new List<Collider2D>(Colliders.Count);
            foreach (var collider in Colliders)
            {
                if (!collider.gameObject.activeSelf) continue;

                if (DistancePointLine(collider.transform.position, ray) < collider.Radius)
                {
                    colliders.Add(collider);
                }
            }
            
            return colliders;

            float DistancePointLine(Vector3 point, Ray ray)
            {
                Vector3 rhs = point - ray.origin;
                float magnitude = ray.direction.magnitude;
                Vector3 lhs = (ray.direction / magnitude);
                float num2 = Mathf.Clamp(Vector3.Dot(lhs, rhs), 0f, lenght);
                var projection = (ray.origin + lhs * num2);
                return (projection - point).magnitude;
            }
        }
    }
}