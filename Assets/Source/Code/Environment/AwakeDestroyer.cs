using UnityEngine;

namespace Source.Code.Environment
{
    public class AwakeDestroyer : MonoBehaviour
    {
        [SerializeField] private Transform[] transformsToDestroy;

        private void Awake()
        {
            foreach (var tr in transformsToDestroy)
            {
                Destroy(tr.gameObject);
            }
            Destroy(this);
        }
    }
}