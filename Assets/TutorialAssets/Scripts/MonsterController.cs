using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace TutorialAssets.Scripts
{
    public enum MonsterState
    {
        Wander = 0,
        Queue = 1,
        Attack = 2,
    }

    public enum MonsterType
    {
        Add = 0,
        Multiply = 1,
        Divide = 2,
    }
    
    public class MonsterController : MonoBehaviour
    {

        public MonsterType type;
        public MonsterState state;
        public int points = 0;
        [SerializeField] private float _findNewPositionEvery = 2f;
        [SerializeField] private float _maxMoveDistance = 10f;

        private float _wanderTimer = 0;
        private NavMeshAgent _agent;
        
        // Start is called before the first frame update
        void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.enabled = false;
            
            //add random factor when spawned as to visually stagger finding move positions
            var halfNewPositionEvery = _findNewPositionEvery;
            _findNewPositionEvery += Random.Range(-halfNewPositionEvery, halfNewPositionEvery);
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case MonsterState.Wander:
                    _agent.enabled = true;
                    _wanderTimer += Time.deltaTime;

                    if (_wanderTimer > _findNewPositionEvery)
                    {
                        FindNewWanderPosition();
                        _wanderTimer = 0;
                    }
                    
                    break;
                case MonsterState.Queue:
                    _agent.enabled = false;
                    
                    break;
                case MonsterState.Attack:
                    _agent.enabled = false;
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void FindNewWanderPosition()
        {
            var newPos = new Vector3(Random.Range(-_maxMoveDistance, _maxMoveDistance), 0, Random.Range(-_maxMoveDistance, _maxMoveDistance));
            _agent.SetDestination(transform.position + newPos);
        }
    }
}
