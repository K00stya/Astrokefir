using UnityEngine;

namespace CustomPhysics
{
    public class CircleCollider2D : Collider2D
    {
        public Vector2 Pivot = new Vector2(0,0);
        public float Radius = 0.5f;
    }
}