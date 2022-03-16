using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject agentPrefab;

    [SerializeField]
    private int numberOfAgentsToSpawn = 16;

    [SerializeField]
    private Color flockColor = Color.red;

    [SerializeField]
    private SimpleInfiniteArea infiniteArea;

    public List<FlockingGameObject> Agents { get; private set; } = new List<FlockingGameObject>();

    private void Start()
    {
        float spawnRadius = GetComponent<MeshRenderer>().bounds.extents.x;

        for(int i = 0; i < numberOfAgentsToSpawn; ++i)
        {
            var agentGo = Instantiate(agentPrefab);

            agentGo.transform.position = new Vector3(
                transform.position.x + Random.Range(-spawnRadius, spawnRadius),
                0.0f,
                transform.position.z + Random.Range(-spawnRadius, spawnRadius));

            var agent = agentGo.GetComponent<FlockingGameObject>();

            agent.Spawner = this;
            agent.Color = flockColor;
            Agents.Add(agent);
        }

        if(infiniteArea != null)
        {
            infiniteArea.ForceObjectsUpdate();
        }
    }
}
