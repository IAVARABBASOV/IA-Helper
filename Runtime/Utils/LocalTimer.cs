using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace IA.Utils
{
    public class LocalTimer : Singleton<LocalTimer>
    {
        [System.Serializable]
        public struct TimeData
        {
            //...
            public string datetime;
            public string day_of_week;
            public string utc_datetime;

            public override string ToString()
            {
                return "datetime: " + datetime + ", day_of_week: " + day_of_week + ", utc_datetime: " + utc_datetime;
            }
            //..
        }

        private Coroutine localTimerCoroutine;
        private Coroutine serverTimeCallCoroutine;

        private const string DATETIME_API_URL = "https://worldtimeapi.org/api/ip";

        public delegate void TimerCallback(DateTime _localTimer);
        public static event TimerCallback OnTimerUpdate;

        private UnityEvent onCurrentTimeLoaded;

        public static DateTime Current_DateTime;
        public static TimeData RequestResult;

        public static bool HasRequestResult => !RequestResult.IsNull();

        [SerializeField] private int maxConnectionAttemptsCount = 5;
        private int connectionAttemptsCount;

        private const string applicationStartDateKey = "APP_START_DATE_DAY_KEY";

        public static void Setup()
        {
            /// Create the Event data
            Instance.onCurrentTimeLoaded = new UnityEvent();

            /// Try save the Start Date time of App on Current time Loading completed
            Instance.onCurrentTimeLoaded.AddListener(TrySaveApplicationStartDate);

            /// Play local timer Counter on Current Time Loading Completed
            Instance.onCurrentTimeLoaded.AddListener(Instance.PlayLocalTimer);

            /// Get Date time from Server or use Local Time
            Instance.CallLoadCurrentDateTimeCoroutine();
        }

        /// <summary>
        /// Save the application start date
        /// </summary>
        public static void TrySaveApplicationStartDate()
        {
            string savedData = PlayerPrefs.GetString(applicationStartDateKey, "default");

            if (savedData == "default")
            {
                SaveDateTimeToPlayerPrefs(applicationStartDateKey, Current_DateTime);
            }
        }

        public static string SaveDateTimeToPlayerPrefs(string _playerPrefsKey, DateTime _dateTime)
        {
            if (_dateTime.IsNull())
            {
                PlayerPrefs.SetString(_playerPrefsKey, "default");
                return string.Empty;
            }

            string dateTimeDataAsString = _dateTime.ToString("MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            PlayerPrefs.SetString(_playerPrefsKey, dateTimeDataAsString);

            return dateTimeDataAsString;
        }

        public static (DateTime, bool) GetDateTimeFromPlayerPrefs(string _playerPrefsKey)
        {
            string dateTimeDataAsString = PlayerPrefs.GetString(_playerPrefsKey, "default");

            if (dateTimeDataAsString == "default")
            {
                return (default, false);
            }

            DateTime dateTimeData = DateTime.ParseExact(dateTimeDataAsString, "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            return (dateTimeData, true);
        }

        /// <summary>
        /// Return day of the which that calculate application since download
        /// </summary>
        /// <returns></returns>
        public static int GetDay()
        {
            (DateTime savedTime, bool hasData) applicationStartTime = GetDateTimeFromPlayerPrefs(applicationStartDateKey);
            TimeSpan difference = Current_DateTime - (applicationStartTime.hasData ? applicationStartTime.savedTime : Current_DateTime);

            /*        Debug.LogError($"<color=cyan>applicationStartTime: {applicationStartTime}</color>");
                    Debug.LogError($"<color=cyan>difference: {difference}</color>");
                    Debug.LogError($"<color=cyan>Current_DateTime: {Current_DateTime}</color>");
            */
            return difference.Days + 1;
        }

        #region Local TIMER

        public void PlayLocalTimer()
        {
            ///// Stop Coroutine if it has
            if (localTimerCoroutine != null) StopCoroutine(localTimerCoroutine);

            /// Create new Coroutine
            localTimerCoroutine = StartCoroutine(LocalTimerRoutine());
        }

        private IEnumerator LocalTimerRoutine()
        {
            /// Add 1 Sec to Local Timer
            UpdateTimer();

            /// Wait 1 Sec
            yield return new WaitForSecondsRealtime(1.0f);

            PlayLocalTimer();
        }

        private void UpdateTimer()
        {
            /// Increase local DateTime
            Current_DateTime = Current_DateTime.AddSeconds(1);

            //    Debug.LogError($"Play Local Timer or Add second: {Current_DateTime}");

            OnTimerUpdate?.Invoke(Current_DateTime);
        }

        #endregion

        #region Get Time
        private void CallLoadCurrentDateTimeCoroutine()
        {
            if (serverTimeCallCoroutine != null) StopCoroutine(serverTimeCallCoroutine);

            serverTimeCallCoroutine = StartCoroutine(LoadCurrentDateTimeRoutine());
        }

        private static TimeData GetTimeDataFromString(string webRequestResult)
        {
            string[] lines = webRequestResult.Split('\n');

            string dateTime = string.Empty;
            string day_Of_Week = string.Empty;
            string utc_dateTime = string.Empty;

            foreach (string line in lines)
            {
                string[] parts = line.Split(':');

                switch (parts[0])
                {
                    case "datetime":
                        dateTime = parts[1].Trim();
                        break;

                    case "day_of_week":
                        day_Of_Week = parts[1].Trim();
                        break;

                    case "utc_datetime":
                        utc_dateTime = parts[1].Trim();
                        break;
                }
            }

            TimeData timeData = new TimeData
            {
                datetime = dateTime,
                day_of_week = day_Of_Week,
                utc_datetime = utc_dateTime
            };

            return timeData;
        }

        public static bool IsJson(string str)
        {
            try
            {
                JToken.Parse(str);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public IEnumerator LoadCurrentDateTimeRoutine()
        {
            /// Create url
            UnityWebRequest webRequest = UnityWebRequest.Get(DATETIME_API_URL);

            /// Set Default DateTime to Local
            Current_DateTime = DateTime.UtcNow;

            /// Send Web Request to Server
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                /// Result is Success!
                case UnityWebRequest.Result.Success:
                    {
                        if (webRequest != null && webRequest.downloadHandler != null)
                        {
                            string webRequestResult = webRequest.downloadHandler.text;

                            if (!string.IsNullOrEmpty(webRequestResult) && !string.IsNullOrWhiteSpace(webRequestResult))
                            {
                                bool isJsonFile = IsJson(webRequestResult);

                                if (isJsonFile)
                                {
                                    RequestResult = JsonUtility.FromJson<TimeData>(webRequestResult);
                                }
                                else
                                {
                                    /// Result from Server
                                    RequestResult = GetTimeDataFromString(webRequestResult);
                                }

                                /// Get as UTC data If Result is not null
                                Current_DateTime = ParseDateTime(RequestResult.utc_datetime);

                                onCurrentTimeLoaded?.Invoke();

                                connectionAttemptsCount = 0;

                                //Debug.Log($"<color=green>Request Result: {Current_DateTime}</color>");
                            }
                            else
                            {
                                RequestResult = new TimeData
                                {
                                    datetime = DateTime.Now.ToString(),
                                    day_of_week = DateTime.Now.DayOfWeek.ToString(),
                                    utc_datetime = DateTime.UtcNow.ToString()
                                };

                                //Debug.Log($"<color=red>RequestResult is NULL</color>");
                            }
                        }
                    }
                    break;

                /// Result is Fail!
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.ProtocolError:
                case UnityWebRequest.Result.DataProcessingError:
                    {
                        OnServerTimeConnectionError();
                    }
                    break;
            }

            //Debug.LogError($"<color=orange>Server time Connection webRequest Result: {webRequest.result}</color>");

            onCurrentTimeLoaded?.Invoke();

            ///// There is no connection
            //if (connectionAttemptsCount >= maxConnectionAttemptsCount)
            //{
            //    onCurrentTimeLoaded?.Invoke();
            //}
        }

        private void OnServerTimeConnectionError()
        {
            if (connectionAttemptsCount < maxConnectionAttemptsCount)
                StartCoroutine(TryConnectionAttemptRoutine());
        }

        private IEnumerator TryConnectionAttemptRoutine()
        {
            yield return new WaitForSeconds(1.0f);
            CallLoadCurrentDateTimeCoroutine();
            connectionAttemptsCount++;
        }

        public static DateTime ParseDateTime(string datetime)
        {
            //match 0000-00-00
            string date = Regex.Match(datetime, @"^\d{4}-\d{2}-\d{2}").Value;

            //match 00:00:00
            string time = Regex.Match(datetime, @"\d{2}:\d{2}:\d{2}").Value;

            return DateTime.Parse(string.Format("{0} {1}", date, time));
        }

        #endregion
    }
}
