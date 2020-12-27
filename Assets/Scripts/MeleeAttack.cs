using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack 
{
    public State playerState;
    public string transitionCondition;
    public float duration;

    public MeleeAttack(State _playerState, string _transitionCondition, float _duration)
    {
        playerState = _playerState;
        transitionCondition = _transitionCondition;
        duration = _duration;
    }

}
