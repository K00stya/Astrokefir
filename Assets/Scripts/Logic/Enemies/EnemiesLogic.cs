using System.Linq;
using UnityEngine;
using Astrokefir.States;
using Astrokefir.View;
using CircleCollider2D = CustomPhysics.CircleCollider2D;
using Collider2D = CustomPhysics.Collider2D;

namespace Astrokefir
{
    public struct EnemiesBattleSceneInfo
    {
        public AsteroidView[] Asteroids;
        public TarelkaView[] Tarelki;
        public ShipView ShipView;
        public (Vector2, Vector2) SpawnBorders;
    }

    public class EnemiesLogic
    {
        private EnemiesPool _asteroids;
        private EnemiesPool _tarelki;
        private ShipView _shipView;

        public void SetEnemies(EnemiesBattleSceneInfo enemiesBattleInfo)
        {
            _asteroids = new EnemiesPool()
            {
                SpawnBorders = enemiesBattleInfo.SpawnBorders,
                MinSpeed = 1f,
                MaxSpeed = 2f,
            };
            _asteroids.AddEnemiesToPool<AsteroidState>(enemiesBattleInfo.Asteroids, OnAsteroidCollide);

            _tarelki = new EnemiesPool()
            {
                SpawnBorders = enemiesBattleInfo.SpawnBorders,
                MinSpeed = 1f,
                MaxSpeed = 3f,
            };
            _tarelki.AddEnemiesToPool<TarelkaState>(enemiesBattleInfo.Tarelki, OnTarelkaCollide);

            _shipView = enemiesBattleInfo.ShipView;
        }

        public void UpdateEnemiesLogic(float deltaTime)
        {
            UpdateAsteroids(deltaTime);
            UpdateTaralki(deltaTime);
        }

        public void ResetEnemies()
        {
            foreach (var ast in _asteroids.GetActivesEnemies().ToArray())
            {
                _asteroids.ReleaseEnemy(ast);
            }

            foreach (var tar in _tarelki.GetActivesEnemies().ToArray())
            {
                _tarelki.ReleaseEnemy(tar);
            }
        }

        private void UpdateAsteroids(float deltaTime)
        {
            //spawn asteroid
            _asteroids.TimerSpawn -= deltaTime;
            if (_asteroids.GetAndSpawnEnemyWithDelay(out var enemy))
            {
                var ast = (AsteroidState) enemy;
                ast.Breakable = true;
                ast.View.transform.localScale = Vector3.one;
                ast.View.GetComponent<CircleCollider2D>().Radius = 0.5f;
            }

            //movement asteroids
            foreach (var asteroid in _asteroids.GetActivesEnemies())
            {
                asteroid.View.transform.position += asteroid.View.transform.up * (asteroid.Speed * deltaTime);
                AstrokefirGameBattleLogic.CheckTeleportBorders(asteroid.View.transform);
            }
        }

        private void UpdateTaralki(float deltaTime)
        {
            //novaya tarelka (letaet)
            _tarelki.TimerSpawn -= deltaTime;
            if (_tarelki.GetAndSpawnEnemyWithDelay(out var tar))
            {
            }

            //movement
            foreach (var tarelka in _tarelki.GetActivesEnemies())
            {
                var dir = _shipView.transform.position - tarelka.View.transform.position;
                tarelka.View.transform.position += dir.normalized * (tarelka.Speed * deltaTime);
            }
        }

        private void OnAsteroidCollide(EnemyState state, Collider2D other)
        {
            if (other.GetComponent<BulletView>() || other.GetComponent<LaserView>())
            {
                AstrokefirGameBattleLogic.Score++;
                _asteroids.ReleaseEnemy(state);
                var asteroid = (AsteroidState) state;

                if (asteroid.Breakable)
                {
                    var position = asteroid.View.transform.position;
                    int quantity = Random.Range(2, 4); //2-3
                    float newScale = 1f / quantity;
                    float newColliderRadius = asteroid.View.GetComponent<CircleCollider2D>().Radius / quantity;
                    float newSpeed = asteroid.Speed * (quantity * 0.5f);
                    for (int i = 0; i < quantity; i++)
                    {
                        var ast = (AsteroidState) _asteroids.GetAndSpawnEnemy();
                        ast.Breakable = false;
                        ast.Speed = newSpeed;
                        ast.View.transform.position = position;
                        ast.View.transform.localScale = new Vector3(newScale, newScale, newScale);
                        ast.View.GetComponent<CircleCollider2D>().Radius = newColliderRadius;
                    }
                }
            }
        }

        private void OnTarelkaCollide(EnemyState enemyState, Collider2D other)
        {
            if (other.GetComponent<BulletView>() || other.GetComponent<LaserView>())
            {
                AstrokefirGameBattleLogic.Score++;
                _tarelki.ReleaseEnemy(enemyState);
            }
        }
    }
}