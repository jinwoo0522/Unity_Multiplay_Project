using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoint;

    public void SpawnGoblins()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            return;

        if (spawnPoint.Length < 3)
        {
            Debug.LogError("고블린 소환에 필요한 스폰 포인트가 3개 미만입니다.");
            return;
        }

        List<Transform> availablePoints = new List<Transform>(spawnPoint);
        for (int i = 0; i < 3; i++)
        {
            int iIndex = Random.Range(0, availablePoints.Count);
            Transform point = availablePoints[iIndex];
            availablePoints.RemoveAt(iIndex);

            Goblin goblin = GameManager.Instance.objectPoolManager.Get<Goblin>(
                NetworkObjectType.GOBLIN, point.position, point.rotation);
            goblin.GetComponent<NetworkObject>().Spawn();
        }
    }

    private void Start()
    {
        SpawnGoblins();
    }
}
