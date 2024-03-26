using UnityEngine;

namespace IA.Utils
{
    public class DisableOrRemove : MonoBehaviour
    {
        public void DisableThisGO()
        {
            gameObject.SetActive(false);
        }

        public void RemoveThisGO()
        {
            Destroy(this.gameObject);
        }
    }
}