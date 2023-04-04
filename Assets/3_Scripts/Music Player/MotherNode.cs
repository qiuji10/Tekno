using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotherNode : MonoBehaviour
{
    public List<TeleportNode> childNode = new List<TeleportNode>();
    private float chargeLevel = 0;
}
