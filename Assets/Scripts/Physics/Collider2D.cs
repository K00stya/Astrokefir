using System;
using UnityEngine;

namespace CustomPhysics
{
    public class Collider2D : MonoBehaviour
    {
        public Action<Collider2D>  OnCollision;
    }
}
