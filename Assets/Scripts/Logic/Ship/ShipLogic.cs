using System;
using UnityEngine;
using Astrokefir.Common;
using Astrokefir.States;
using Astrokefir.View;
using Collider2D = CustomPhysics.Collider2D;
using Rigidbody2D = CustomPhysics.Rigidbody2D;

namespace Astrokefir
{
    public struct ShipBattleSceneInfo
    {
        public ShipView ShipView;
        public LaserView LaserView;
        public BulletView[] Bullets;
    }

    public struct ShipControlInput
    {
        public bool MoveInputPressed;
        public float Spin;
        public float MoveForce;
    }

    public class ShipLogic
    {
        public GunsLogic GunsLogic = new GunsLogic();
        public Action OnShipDie;
        
        private ShipState _shipState;
        private ShipControlInput _input;
        
        private Action<string> _onShipPositionChange;
        private Action<string> _onAngleChange;
        private Action<string> _onInstantaneousSpeedChange;

        public void Start(ShipBattleSceneInfo ship)
        {
            DebugUI.OnDataChange("ShipPos", ref _onShipPositionChange);
            DebugUI.OnDataChange("ShipAngle", ref _onAngleChange);
            DebugUI.OnDataChange("InstSpeed", ref _onInstantaneousSpeedChange);
            
            _shipState = new ShipState() {View = ship.ShipView};
            _shipState.View.gameObject.SetActive(true);
            _shipState.View.GetComponent<Collider2D>().OnCollision += (collider) =>
            {
                OnShipCollide(_shipState.View.GetComponent<Collider2D>(), collider);
            };

            GunsLogic.Start(ship.LaserView, ship.Bullets, ship.ShipView);
        }

        public void UpdateInput(ShipControlInput shipControlInput)
        {
            _input = shipControlInput;
        }

        public void UpdateShipLogic(float deltaTime)
        {
            var rigidbody = _shipState.View.GetComponent<Rigidbody2D>(); //can be cashed
            ShipMovement();
            AstrokefirGameBattleLogic.CheckTeleportBorders(_shipState.View.transform);
            DebugInfoUpdate();

            GunsLogic.UpdateBullets(deltaTime);
            GunsLogic.UpdateLaser(deltaTime);
            
            
            void ShipMovement()
            {
                if (!_input.MoveInputPressed) return;
                
                //movement
                var velocity = _shipState.View.transform.up *
                               (_shipState.MoveAcceleration * _input.MoveForce * deltaTime);
                rigidbody.AddForce(velocity);
                if (rigidbody.Velocity.magnitude >= _shipState.MaxMoveSpeed)
                {
                    rigidbody.Velocity = rigidbody.Velocity.normalized * _shipState.MaxMoveSpeed;
                }

                //rotation
                float rotate = _input.Spin * _shipState.RotationAcceleration * deltaTime;
                rigidbody.AddAngularForce(new Vector3(0, 0, -rotate));
                if (rigidbody.AngularVelocity.magnitude >= _shipState.MaxRotationSpeed)
                {
                    rigidbody.AngularVelocity = rigidbody.AngularVelocity.normalized * _shipState.MaxRotationSpeed;
                }
            }

            void DebugInfoUpdate()
            {
                _onInstantaneousSpeedChange?.Invoke(((Vector2) rigidbody.Velocity).ToString());
                var position = (Vector2) (_shipState.View.transform.position);
                _onShipPositionChange?.Invoke(position.ToString());
                _onAngleChange?.Invoke(_shipState.View.transform.eulerAngles.z.ToString());
            }
        }

        public void ResetShip()
        {
            _shipState.View.gameObject.SetActive(true);
            _shipState.View.transform.position = Vector3.zero;
            _shipState.View.transform.rotation = Quaternion.Euler(Vector3.zero);
            _shipState.View.GetComponent<Rigidbody2D>().Velocity = Vector3.zero;

            GunsLogic.ResetGuns();
        }

        private void OnShipCollide(Collider2D ship, Collider2D other)
        {
            if (other.GetComponent<AsteroidView>() || other.GetComponent<TarelkaView>())
            {
                _shipState.View.gameObject.SetActive(false);
                OnShipDie?.Invoke();
            }
        }
    }
}