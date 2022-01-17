using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.IO;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyAnimation))]
public class WanderEnemy : Interactable
{
    private static readonly string s_kLoadTableFailed = "Loot Table failed to load.";

    [Tooltip("The point in the world that the enemy will wander around.")]
    [SerializeField] private Transform m_wanderCenter = null;

    [Tooltip("The maximum distance the enemy will wander from the world point.")]
    [SerializeField] private float m_wanderRadius = 10.0f;

    [Tooltip("The amount of time that must pass before the enemy will wander again.")]
    [SerializeField] private float m_wanderCooldown = 0.0f;

    [Tooltip("The amount of time that must pass before the enemy will be destroyed after recieving the death event.")]
    [SerializeField] private float m_deathWaitTime = 1.0f;

    // The time remaining until the next wander action.
    private float m_currentWanderTime = 0.0f;

    [Tooltip("The text file with the table data containing the possible loot this enemy can drop.")]
    [SerializeField] private TextAsset m_lootTableText = null;

    // The table loaded from the text asset.
    private LootTable m_lootTable = null;

    // The cached nav mesh agent of this enemy.
    private NavMeshAgent m_agentComponent = null;

    // This enemies cached game object.
    private GameObject m_thisObject = null;

    // This enemies cached transform.
    private Transform m_transform = null;

    // The event that this enemy will fire on attack.
    private EventData m_attackedEventData = new EventData();

    private bool m_isDead = false;

    private void Awake()
    {
        if (m_agentComponent == null)
            m_agentComponent = GetComponent<NavMeshAgent>();

        m_thisObject = this.gameObject;
        m_transform = transform;
        m_attackedEventData.m_targetObject = m_thisObject;

        LoadLootTable();
    }

    private void Update()
    {
        if (m_isDead)
            return;

        m_currentWanderTime -= Time.deltaTime;

        if (m_currentWanderTime <= 0.0f)
        {
            Wander();
            m_currentWanderTime = m_wanderCooldown;
        }
    }

    private void LoadLootTable()
    {
        if (m_lootTableText == null)
            return;

        m_lootTable = new LootTable();

        string jsonData = m_lootTableText.text;

        JsonUtility.FromJsonOverwrite(jsonData, m_lootTable);

        if (m_lootTable == null)
        {
            Debug.LogError(s_kLoadTableFailed);
            return;
        }

        m_lootTable.InitializeLootTable();
    }

    // Called when the player interacts with this collectable.
    protected override void Interact(GameObject interactor)
    {
        TriggerEnemyAttackedEvent(interactor);
        m_isDead = true;

        StartCoroutine(WaitTimeUntilDeath(m_deathWaitTime));
    }

    // Wait for given seconds and destroy this enemy, while dropping loot.
    // This isn't ideal, as this is purely for timing animations. In reality
    // death should be triggered by a series of events, especially end of animation
    // events.
    private IEnumerator WaitTimeUntilDeath(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        DropRandomLoot();
        GameObject.Destroy(m_thisObject);
    }

    // Makes the enemy route to a random position within the radius.
    private void Wander()
    {
        if (m_wanderCenter == null)
            return;

        Vector3 wanderPos = Random.insideUnitSphere * m_wanderRadius + m_wanderCenter.position;

        NavMesh.SamplePosition(wanderPos, out NavMeshHit navMeshHit, m_wanderRadius, -1);

        // Wander within this position and radius.
        m_agentComponent.SetDestination(navMeshHit.position);
    }

    // Spawn a random item from the loot table using a weighted random.
    private void DropRandomLoot()
    {
        if (m_lootTable == null)
            return;

        Item drop = m_lootTable.GetWeightedRandomDrop();
        if (drop != null && drop.m_itemGameObject != null && m_transform != null)
        {
            GameObject instance = GameObject.Instantiate(drop.m_itemGameObject, m_transform.position, drop.m_itemGameObject.transform.rotation);
            Collectable collectable = instance.GetComponent<Collectable>();
            if (collectable == null)
                return;

            collectable.item.m_itemDropRate = drop.m_itemDropRate;
            collectable.item.m_itemGameObject = drop.m_itemGameObject;
            collectable.item.m_itemInventorySprite = drop.m_itemInventorySprite;
        }
    }

    // Helper for triggering the enemy attacked event.
    private void TriggerEnemyAttackedEvent(GameObject interactor)
    {
        if (interactor != null)
            m_attackedEventData.m_triggerObject = interactor;
        else
            m_attackedEventData.m_triggerObject = null;

        EventManager.TriggerEvent(EventID.s_kCharacterAttackedEvent, m_attackedEventData);
    }
}
