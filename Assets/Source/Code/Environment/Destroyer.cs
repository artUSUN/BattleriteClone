using System.Collections;
using UnityEngine;

namespace Source.Code.Environment
{
    public class Destroyer : MonoBehaviour
    {
        public void DestroyAfterFallingUnderground(float getDownAfter, float getDownSpeed, float destroyDelay)
        {
            StartCoroutine(DestroyAfterFallingUndergroundCoroutine(getDownAfter, getDownSpeed, destroyDelay));
        }

        private IEnumerator DestroyAfterFallingUndergroundCoroutine(float getDownAfter, float getDownSpeed, float destroyDelay)
        {
            yield return new WaitForSeconds(getDownAfter);
            var tr = transform;
            float timer = 0;
            while (timer < destroyDelay)
            {
                tr.Translate(Vector3.down * getDownSpeed * Time.deltaTime);
                timer += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}