using UnityEngine;
using UnityEngine.Events;

namespace IA.Utils
{
    public class SlowMotion
    {
        private bool isSlowmotionOnPeek;
        private float realtime;

        public float slowdownFactor = 0.05f;
        public float slowMotionValue = 1f;

        public float SlowMotionLength = 2f;

        public UnityAction OnSlowMotionCompleted;

        public bool IsSlowMotion { get; set; }

        public SlowMotion()
        {
            realtime = 0;

            IsSlowMotion = true;

            isSlowmotionOnPeek = false;
        }

        ~SlowMotion()
        {
            SetGameTime(1);
        }

        public void Update()
        {
            if (!isSlowmotionOnPeek)
            {
                if (Time.timeScale <= slowdownFactor)
                {
                    isSlowmotionOnPeek = true;
                }
                else
                {
                    float _tempTimeScale = Time.timeScale;
                    _tempTimeScale -= slowMotionValue * Time.unscaledDeltaTime;

                    Time.timeScale = Mathf.Clamp(_tempTimeScale, slowdownFactor, 1f);
                    Time.fixedDeltaTime = Time.timeScale * 0.02f;
                }
            }
            else
            {
                realtime += Time.unscaledDeltaTime;

                if (realtime >= SlowMotionLength)
                {
                    SetGameTime(1f);
                    IsSlowMotion = false;

                    OnSlowMotionCompleted?.Invoke();
                }
            }
        }

        private void SetGameTime(float gameTime)
        {
            Time.timeScale = gameTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public class SlowMotionComponent : Singleton<SlowMotionComponent>
    {
        [SerializeField] private float slowdownFactor = 0.05f;
        [SerializeField] private float slowMotionValue = 1f;
        public float SlowMotionLength = 2f;

        private SlowMotion slowMotion;

        private void Start()
        {
            SetGameTime(1f);
        }

        protected void Update()
        {
            if(slowMotion != null)
            {
                slowMotion.Update();
            }
        }

        public void DoSlowMotion(UnityAction onCompleted)
        {
            slowMotion = new SlowMotion()
            {
                OnSlowMotionCompleted = onCompleted,
                slowdownFactor = slowdownFactor,
                slowMotionValue = slowMotionValue,
                SlowMotionLength = SlowMotionLength
            };
        }

        public void MakeSlowMotion(float slowMotionLength, UnityAction onCompleted = null)
        {
            slowMotion = new SlowMotion()
            {
                OnSlowMotionCompleted = onCompleted,
                SlowMotionLength = slowMotionLength,
                slowMotionValue = slowMotionValue,
                slowdownFactor = slowdownFactor
            };
        }

        public void StopSlowMotion()
        {
            SetGameTime(1f);
        }

        private void SetGameTime(float gameTime)
        {
            Time.timeScale = gameTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }
}
