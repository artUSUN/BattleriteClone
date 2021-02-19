using UnityEngine;

namespace Source.Code.Environment
{
    public class SelfDestroyer : MonoBehaviour
    {
        public void Initialize(float lifeTimeDuration)
        {
            Destroy(gameObject, lifeTimeDuration);
        }
    }
}