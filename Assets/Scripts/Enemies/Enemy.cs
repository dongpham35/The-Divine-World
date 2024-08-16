using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public void CheckDistance();

    public void BeAttack(int attack_damage, int amor_penetraction);

    public void MoveNorState();

    public void HitPLayer();
}