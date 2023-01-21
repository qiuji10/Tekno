using UnityEngine;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    [Button]
    public void ResetPlayerLife()
    {
        PlayerStatus.life = 0;
    }
}