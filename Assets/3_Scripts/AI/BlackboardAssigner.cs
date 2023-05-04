using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardAssigner : MonoBehaviour
{
    GlobalBlackboard board;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        board = GetComponent<GlobalBlackboard>();
        board.SetVariableValue("Player", player);
        board.SetVariableValue("PlayerTransform", player.transform);
    }
}
