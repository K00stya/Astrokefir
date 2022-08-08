using System;
using System.Collections.Generic;
using UnityEngine;
using Astrokefir.View;
using Collider2D = CustomPhysics.Collider2D;

namespace Astrokefir.States
{
    public class BulletsGunState : WeaponState
    {
        public float Speed = 10f;
        private Queue<MonoBehaviour> _pool = new Queue<MonoBehaviour>();
        private List<MonoBehaviour> _active = new List<MonoBehaviour>();

        public bool GetBullet(out MonoBehaviour bullet)
        {
            if (TimerReload <= 0 && _pool.Count > 0)
            {
                TimerReload = ReloadTime;
                bullet = _pool.Dequeue();
                _active.Add(bullet);
                bullet.gameObject.SetActive(true);
                return true;
            }

            bullet = null;
            return false;
        }
        
        public void AddBulletsToPool(BulletView[] bullets, Action<BulletView, Collider2D> onCollision)
        {
            foreach (var bullet in bullets)
            {
                bullet.GetComponent<Collider2D>().OnCollision += (collider) => { onCollision(bullet, collider); };
                _pool.Enqueue(bullet);
            }
        }
        
        public void Release(MonoBehaviour bullet)
        {
            _active.Remove(bullet);
            _pool.Enqueue(bullet);
            bullet.gameObject.SetActive(false);
        }
        
        public MonoBehaviour[] GetActives()
        {
            return _active.ToArray();
        }
    }
}