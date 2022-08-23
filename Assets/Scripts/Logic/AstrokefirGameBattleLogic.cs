using Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Astrokefir
{
    public class AstrokefirGameBattleLogic
    {
        public static int Score;
        private static (Vector2, Vector2) _borders;

        public ShipLogic ShipLogic = new ShipLogic();
        public EnemiesLogic EnemiesLogic = new EnemiesLogic();

        private EndGameMenu _endGameMenu;

        public void StartBattle(ShipBattleSceneInfo ship, EnemiesBattleSceneInfo enemiesBattle, EndGameMenu endGameMenu,
            (Vector2, Vector2) borders)
        {
            ShipLogic.OnShipDie += BattleEnd;
            ShipLogic.Start(ship);
            EnemiesLogic.SetEnemies(enemiesBattle);

            _endGameMenu = endGameMenu;
            _borders = borders;
        }

        public void SetInput(InputAction firsWeapon, InputAction secondWeapon)
        {
            firsWeapon.performed += ShipLogic.TryShootBullet;
            secondWeapon.performed += ShipLogic.TryShootLaser;
        }

        public void UpdateInput(ShipControlInput shipControlInput)
        {
            ShipLogic.UpdateInput(shipControlInput);
        }

        public void GameLogicUpdate(float deltaTime)
        {
            ShipLogic.UpdateShipLogic(deltaTime);
            EnemiesLogic.UpdateEnemiesLogic(deltaTime);
        }

        public void StartNewBattle()
        {
            _endGameMenu.gameObject.SetActive(false);
            ShipLogic.ResetShip();
            EnemiesLogic.ResetEnemies();
            Score = 0;
        }

        public void BattleEnd()
        {
            _endGameMenu.gameObject.SetActive(true);
            _endGameMenu.Score.text = Score.ToString();
        }

        public static void CheckTeleportBorders(Transform transformView)
        {
            var position = transformView.position;
            Vector3 newPosition = position;
            if (position.x > _borders.Item2.x)
            {
                newPosition = new Vector3(_borders.Item1.x, position.y);
            }

            if (position.x < _borders.Item1.x)
            {
                newPosition = new Vector3(_borders.Item2.x, position.y);
            }

            if (position.y > _borders.Item2.y)
            {
                newPosition = new Vector3(position.x, _borders.Item1.y);
            }

            if (position.y < _borders.Item1.y)
            {
                newPosition = new Vector3(position.x, _borders.Item2.y);
            }

            transformView.position = newPosition;
        }

        public static bool CrossBorders(Vector2 position)
        {
            return position.x > _borders.Item2.x || position.x < _borders.Item1.x ||
                   position.y > _borders.Item2.y || position.y < _borders.Item1.y;
        }

    }
}