using UnityEngine;
using CustomPhysics;
using Astrokefir.Common;
using Astrokefir.View;
using CircleCollider2D = CustomPhysics.CircleCollider2D;
using Rigidbody2D = CustomPhysics.Rigidbody2D;

namespace Astrokefir
{
    public class AstrokefirCompositionRoot : MonoBehaviour
    {
        [Header("Prefabs")] public ShipView ShipPrefab;
        public BulletView BulletPrefab;
        public LaserView LaserPrefab;
        public TarelkaView TarelkaPrefab;
        public AsteroidView AsteroidPrefab;

        [Header("UI")] public EndGameMenu EndGameMenu;

        private PhysicsSimulation _physicsSimulation;
        private InputActions _inputActions;

        private AstrokefirGameBattleLogic _battleLogic;

        private void Awake()
        {
            _physicsSimulation = new PhysicsSimulation();
            _inputActions = new InputActions();
            _inputActions.Enable();

            _battleLogic = new AstrokefirGameBattleLogic();
            _inputActions.Player.FireBullet.performed += _battleLogic.ShipLogic.GunsLogic.OnBulletFireInput;
            _inputActions.Player.FireLaser.performed += _battleLogic.ShipLogic.GunsLogic.OnLaserFireInput;

            EndGameMenu.gameObject.SetActive(false);
            EndGameMenu.PlayAgainButton.onClick.AddListener(_battleLogic.StartNewBattle);

        }

        private void Start()
        {
            var shipView = SpawnObjects(ShipPrefab, 1)[0];
            var borders = ScreenWorldSpaceBorders();
            var shipInfo = new ShipBattleSceneInfo()
            {
                ShipView = shipView,
                LaserView = SpawnObjects(LaserPrefab, 1)[0],
                Bullets = SpawnObjects(BulletPrefab, 25),
            };

            var enemies = new EnemiesBattleSceneInfo()
            {
                Asteroids = SpawnObjects(AsteroidPrefab, 100),
                Tarelki = SpawnObjects(TarelkaPrefab, 15),
                ShipView = shipView,
                SpawnBorders = borders
            };

            _battleLogic.StartBattle(shipInfo, enemies, EndGameMenu, borders);
        }

        private void Update()
        {
            var pressed = _inputActions.Player.Move.IsPressed();
            var dirInput = _inputActions.Player.Move.ReadValue<Vector2>();
            var shipControl = new ShipControlInput
            {
                MoveInputPressed = pressed, MoveForce = dirInput.y, Spin = dirInput.x,
            };

            _battleLogic.UpdateInput(shipControl);
            _battleLogic.GameLogicUpdate(Time.deltaTime);
        }


        private void FixedUpdate()
        {
            _physicsSimulation.FixedUpdate(Time.fixedDeltaTime);
        }

        private T[] SpawnObjects<T>(T prefab, int quantity = 1) where T : MonoBehaviour
        {
            if (prefab == null)
            {
                Debug.LogError("Object for creating a pool is not specified.");
                return null;
            }

            if (quantity <= 0)
            {
                Debug.LogError($"Creating a objects <= 0 in lenght not success. Creation objects canceled.");
                return null;
            }

            var spawnedObjects = new T[quantity];
            for (int i = 0; i < quantity; i++)
            {
                var obj = Instantiate(prefab);
                obj.gameObject.SetActive(false);
                if (obj.TryGetComponent<CircleCollider2D>(out var collider))
                {
                    PhysicsSimulation.Colliders.Add(collider);
                }

                if (obj.TryGetComponent<Rigidbody2D>(out var rigidbody))
                {
                    PhysicsSimulation.Rigidbodies.Add(rigidbody);
                }

                spawnedObjects[i] = obj;
            }


            return spawnedObjects;
        }

        private (Vector3, Vector3) ScreenWorldSpaceBorders()
        {
            var camera = Camera.main;
            var lowerLeftScreen = new Vector2(0, 0);
            var upperRightScreen = new Vector2(Screen.width, Screen.height);

            var lowerLeft = camera.ScreenToWorldPoint(lowerLeftScreen);
            var upperRight = camera.ScreenToWorldPoint(upperRightScreen);
            return (lowerLeft, upperRight);
        }
    }
}