using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState : MonoBehaviour
{
    public abstract void EnterState(PlayerTestController player);

    public abstract void Update(PlayerTestController player);
}
