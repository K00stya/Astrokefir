namespace Astrokefir.States
{
    public class LaserGunState : WeaponState
    {
        public float LifeTime = 0.5f;
        public float CurrentLifeTime = 0;
        
        public int MaxCharges = 3;
        public int Charges = 3;
        
        public float AddNewChargeTime = 5f;
        public float TimerCharge = 5f;
    }
}