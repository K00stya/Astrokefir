using System;
using System.Collections.Generic;
using UnityEngine;
using Collider2D = CustomPhysics.Collider2D;
using Random = UnityEngine.Random;

namespace Astrokefir.States
{
    public class EnemiesPool
    {
        public (Vector2, Vector2) SpawnBorders;
        
        private Queue<EnemyState> _pool = new Queue<EnemyState>();
        private List<EnemyState> _active = new List<EnemyState>();
        public float DelayBetweenSpawn = 2f;
        public float TimerSpawn = 0;
        
        public float MaxSpeed = 2f;
        public float MinSpeed = 1f;

        public bool GetAndSpawnEnemyWithDelay(out EnemyState enemy)
        {
            if (TimerSpawn <= 0 && _pool.Count > 0)
            {
                TimerSpawn = DelayBetweenSpawn;
                enemy = _pool.Dequeue();
                _active.Add(enemy);
                enemy.View.gameObject.SetActive(true);
                enemy.View.transform.position = GetSpawnPosition();
                enemy.View.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                enemy.Speed = Random.Range(MinSpeed, MaxSpeed);
                return true;
            }

            enemy = null;
            return false;
        }

        public EnemyState GetAndSpawnEnemy()
        {
            var enemy = _pool.Dequeue();
            _active.Add(enemy);
            enemy.View.gameObject.SetActive(true);
            enemy.View.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            return enemy;
        }

        public void ReleaseEnemy(EnemyState enemy)
        {
            _active.Remove(enemy);
            _pool.Enqueue(enemy);
            enemy.View.gameObject.SetActive(false);
        }

        public EnemyState[] GetActivesEnemies()
        {
            return _active.ToArray();
        }

        public void AddEnemiesToPool<T>(MonoBehaviour[] enemies, Action<EnemyState, Collider2D> onCollision) where T: EnemyState, new()
        {
            foreach (var enemy in enemies)
            {
                var enemyState = new T() {View = enemy, Speed = Random.Range(MinSpeed, MaxSpeed)};
                enemyState.View.GetComponent<Collider2D>().OnCollision += (collider) => { onCollision(enemyState, collider); };
                _pool.Enqueue(enemyState);
            }
        }

        private Vector3 GetSpawnPosition()
        {
            Vector3 spawnPosition;
            var randomBool = Random.Range(0, 2) > 0;
            if (Random.Range(0, 2) > 0)
            {
                var x = Random.Range(SpawnBorders.Item1.x, SpawnBorders.Item2.x);
                spawnPosition =
                    randomBool ? new Vector3(x, SpawnBorders.Item1.y) : new Vector3(x, SpawnBorders.Item2.y);
            }
            else
            {
                var y = Random.Range(SpawnBorders.Item1.y, SpawnBorders.Item2.y);
                spawnPosition =
                    randomBool ? new Vector3(SpawnBorders.Item1.x, y) : new Vector3(SpawnBorders.Item2.x, y);
            }

            return spawnPosition;
        }
    }
}