using NodeCanvas.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackboardAssigner : MonoBehaviour
{
    GlobalBlackboard board;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        board = FindObjectOfType<GlobalBlackboard>();
        board.SetVariableValue("Player", player);
        board.SetVariableValue("PlayerTransform", player.transform);
    }
}
