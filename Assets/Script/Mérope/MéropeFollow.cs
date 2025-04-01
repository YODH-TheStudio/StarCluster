using UnityEngine;
using UnityEngine.AI;

public class MéropeFollow : MonoBehaviour
{
    private PlayerScript _player;
    
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float maxDistance = 5.5f;
    [SerializeField] private float minDistance = 2.5f;
    [SerializeField] private float floatHeight = 1.5f;
    [SerializeField] private float floatSpeed = 0.1f;
    
    private NavMeshAgent _agent;
    private Rigidbody _rb;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // follow the player
        if (_player != null)
        {
            // If too far from the player, move towards the player
            Vector3 direction = _player.transform.position - transform.position;
            direction.y = 0; // Keep the y-axis constant to avoid vertical movement
            
            float dist = (_player.transform.position - transform.position).magnitude;
            if(dist > maxDistance)
            {
                // If too far from the player, move towards them
                _agent.SetDestination(_player.transform.position);
                //transform.position += direction.normalized * speed * dist/5 * Time.deltaTime;
            } else if (dist < minDistance)
            {
                // If too close to the player, move away
                transform.position -= direction.normalized * speed * Time.deltaTime;
            }
        }
        else
        {
            Debug.LogWarning("Player not found");
        }
        
        // Move up and down
        Vector3 newPosition = transform.position;
        newPosition.y = Mathf.Sin(Time.time * speed) * floatSpeed + floatHeight;
        transform.position = newPosition;
    }
}
