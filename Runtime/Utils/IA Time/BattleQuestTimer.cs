using UnityEngine;
using System;

namespace IA.Utils
{

    public class BattleQuestTimer : Singleton<BattleQuestTimer>
    {
        public static bool IsTimerRun;

        public DateTime TargetTime { get; private set; }

        public delegate void TimerCallback(TimeSpan _period);
        public static event TimerCallback OnTimerUpdate;
        public static event TimerCallback OnTimerCompleted;

        private const string savedTargetTimeKey = "Target_Time_Saved_KEY";

        public void PlayTimer()
        {
            //  Debug.LogError("Play Timer");

            //DebugExtensions.LogError(, Color.blue);
            "Play Timer".LogError(Color.blue);

            if (!IsTimerRun)
            {
                IsTimerRun = true;

                SetupTargetTime(load: true);

                LocalTimer.OnTimerUpdate += LocalTimer_OnTimerUpdate;
            }
        }

        private void SetupTargetTime(bool load)
        {
            /// Load DateTime from Player Prefs
            (DateTime savedTime, bool hasData) savedTargetDateTime = LocalTimer.GetDateTimeFromPlayerPrefs(savedTargetTimeKey);

            if (load)
            {
                if (savedTargetDateTime.hasData)
                {
                    TargetTime = savedTargetDateTime.savedTime;
                }
                else
                {
                    TargetTime = LocalTimer.Current_DateTime;
                }
            }
            else
            {
                /// If Data is Null
                if (!savedTargetDateTime.hasData)
                {
                    /// Create data 
                    TargetTime = GetNewTargetDateTime();

                    /// Save data to Player Prefs
                    LocalTimer.SaveDateTimeToPlayerPrefs(savedTargetTimeKey, TargetTime);
                }
                else /// There is Data
                {
                    /// Get Current Data
                    TargetTime = savedTargetDateTime.savedTime;
                }
            }
        }

        private DateTime GetNewTargetDateTime()
        {
            return LocalTimer.Current_DateTime.AddDays(1);

            //   return LocalTimer.Current_DateTime.AddMinutes(1);
        }

        private void LocalTimer_OnTimerUpdate(DateTime _localTimer)
        {
            TimeSpan period = TargetTime - _localTimer;

            //  Debug.LogError("Battle Quest Timer Completed Period: " + period);

            if (period > TimeSpan.Zero)
            {
                //  Debug.LogError("Battle Quest Timer Period: " + period);
                OnTimerUpdate?.Invoke(period);
            }
            else
            {
                //  Debug.LogError("Battle Quest Timer Reset: " + period);

                LocalTimer.OnTimerUpdate -= LocalTimer_OnTimerUpdate;

                OnTimerCompleted?.Invoke(period);

                ResetTimer();
            }
        }

        private void ResetTimer()
        {
            IsTimerRun = false;

            LocalTimer.SaveDateTimeToPlayerPrefs(savedTargetTimeKey, default);

            SetupTargetTime(load: false);

            PlayTimer();
        }
    }
}