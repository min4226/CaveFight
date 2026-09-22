using System;
using UnityEngine;

public class HitPointModule : CharacterModule
{
    float _hpCur;
    public float HPCur => _hpCur;

    float _hpMax;
    public float HPMax => _hpMax;
    public sealed override Type RegistrationType => typeof(HitPointModule);

    //public float IncreaseHP(float value);
    //public float DecreaseHP(float value);
    // public float SetHP(float value);
    //public float Damage();
    //public float Heal();
    //public bool FallCheck();
}