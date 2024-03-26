using UnityEngine;

namespace IA.Utils
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }

        public bool IsDontDestroyOnLoad = false;

        protected virtual void Awake()
        {
            // singleton
            if (Instance == null)
            {
                Instance = (T)this;
            }
            else if (Instance != this)
            {
                if (IsDontDestroyOnLoad) Destroy(this.gameObject);
                return;
            }
            if (IsDontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            // singleton end
        }
    }
}