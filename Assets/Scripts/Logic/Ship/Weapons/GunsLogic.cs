using System;
using System.Linq;
using UnityEngine;
using Astrokefir.Common;
using Astrokefir.States;
using Astrokefir.View;
using CustomPhysics;
using Collider2D = CustomPhysics.Collider2D;

namespace Astrokefir
{
    public class GunsLogic
    {
        private LaserGunState _laserGun;
        private BulletsGunState _bulletsGun;
        private ShipView _shipView;

        private Action<string> _onLaserChargesChange;
        private Action<string> _onLaserReloadChange;

        public void Start(LaserView laserView, BulletView[] bullets, ShipView shipView)
        {
            DebugUI.OnDataChange("LasCharge", ref _onLaserChargesChange);
            DebugUI.OnDataChange("LasReload", ref _onLaserReloadChange);

            _bulletsGun = new BulletsGunState()
            {
                Speed = 10f,
                ReloadTime = 0.1f
            };
            _bulletsGun.AddBulletsToPool(bullets, OnBulletCollide);

            _laserGun = new LaserGunState()
            {
                ReloadTime = 1f
            };

            _laserGun.View = laserView;
            _onLaserChargesChange?.Invoke(_laserGun.Charges.ToString());

            _shipView = shipView;
        }

        public void UpdateBullets(float deltaTime)
        {
            _bulletsGun.TimerReload -= deltaTime;
            foreach (var bullet in _bulletsGun.GetActives())
            {
                bullet.transform.position += bullet.transform.up * (_bulletsGun.Speed * deltaTime);

                if (AstrokefirGameBattleLogic.CrossBorders(bullet.transform.position))
                {
                    _bulletsGun.Release(bullet);
                }
            }
        }

        public void UpdateLaser(float deltaTime)
        {
            _laserGun.TimerReload -= deltaTime;
            if (_laserGun.TimerReload < 0)
            {
                _laserGun.TimerReload = 0;
            }

            _onLaserReloadChange?.Invoke(_laserGun.TimerReload.ToString());

            _laserGun.TimerCharge -= deltaTime;
            if (_laserGun.TimerCharge <= 0)
            {
                _laserGun.TimerCharge = _laserGun.AddNewChargeTime;
                if (_laserGun.Charges < _laserGun.MaxCharges)
                {
                    _laserGun.Charges++;
                    _onLaserChargesChange?.Invoke(_laserGun.Charges.ToString());
                }
            }

            _laserGun.CurrentLifeTime += deltaTime;
            if (_laserGun.CurrentLifeTime <= _laserGun.LifeTime && _laserGun.View.gameObject.activeSelf)
            {
                _laserGun.View.transform.position = _shipView.transform.position;
                _laserGun.View.transform.rotation = _shipView.transform.rotation;

                var colliders = PhysicsSimulation.RayCast2D(new Ray(_laserGun.View.transform.position,
                    _laserGun.View.transform.up));
                foreach (var collider in colliders)
                {
                    collider.OnCollision?.Invoke(_laserGun.View.GetComponent<Collider2D>());
                }
            }
            else
            {
                _laserGun.View.gameObject.SetActive(false);
            }
        }

        public void ResetGuns()
        {
            _laserGun.View.gameObject.SetActive(false);
            _laserGun.Charges = _laserGun.MaxCharges;
            _laserGun.CurrentLifeTime = 0;
            _laserGun.TimerCharge = 0;

            foreach (var bul in _bulletsGun.GetActives().ToArray())
            {
                _bulletsGun.Release(bul);
            }

            _bulletsGun.TimerReload = 0;
        }

        public void BulletFire()
        {
            if (_bulletsGun.GetBullet(out var bullet))
            {
                bullet.transform.position = _shipView.transform.position;
                bullet.transform.rotation = _shipView.transform.rotation;
            }
        }

        public void LaserFire()
        {
            if (_laserGun.Charges > 0 && _laserGun.TimerReload <= 0)
            {
                _laserGun.Charges--;
                _onLaserChargesChange?.Invoke(_laserGun.Charges.ToString());
                _laserGun.CurrentLifeTime = 0;
                _laserGun.TimerReload = _laserGun.ReloadTime;
                _laserGun.View.gameObject.SetActive(true);
            }
        }

        private void OnBulletCollide(BulletView bullet, CustomPhysics.Collider2D other)
        {
            if (other.GetComponent<AsteroidView>() || other.GetComponent<TarelkaView>())
            {
                _bulletsGun.Release(bullet);
            }
        }
    }
}