using System;
using System.Text;

namespace IA.Utils
{
    public class Timer
    {
        private DateTime waitingTime;

        public DateTime GetWaitingTime => waitingTime;

        private event Action<DateTime> onTimerUpdateDateTime;
        private event Action<string> onTimerUpdate;
        private event Action onTimerStoped;

        public Timer(TimerProperties properties, bool isAddToNowTime = true,
            Action<string> onTimerUpdate = null, Action onTimerStoped = null,
            Action<DateTime> onTimerDateTime = null)
        {
            this.onTimerUpdate = onTimerUpdate;
            this.onTimerStoped = onTimerStoped;
            this.onTimerUpdateDateTime = onTimerDateTime;

            waitingTime = isAddToNowTime ?
            DateTime.UtcNow.
            AddHours(properties.GetHour).
            AddMinutes(properties.GetMinute).
            AddSeconds(properties.GetSecond).
            AddMilliseconds(properties.GetMilliSecond) : new DateTime(
                                                            properties.GetYear,
                                                            properties.GetMonth,
                                                            properties.GetDay,
                                                            properties.GetHour,
                                                            properties.GetMinute,
                                                            properties.GetSecond,
                                                            properties.GetMilliSecond, DateTimeKind.Utc);
        }

        public Timer Run()
        {
            DateTime timerPeriod = CalculateTimePeriod(waitingTime.Ticks, DateTime.UtcNow.Ticks);

            string periodResult = GetTimerPeriodResult(timerPeriod);

            onTimerUpdate?.Invoke(periodResult);

            onTimerUpdateDateTime?.Invoke(timerPeriod);

            if (timerPeriod.Year == 0 && timerPeriod.Month == 0 && timerPeriod.Day == 0 &&
                timerPeriod.Hour == 0 && timerPeriod.Minute == 0 && timerPeriod.Second < 1)
            {
                onTimerStoped?.Invoke();
                return null;
            }

            return this;
        }

        public static string GetTimerPeriodResult(DateTime timerPeriod)
        {
            StringBuilder result = new StringBuilder();

            if (timerPeriod.Hour > 0) result.Append(GetFormat(timerPeriod.Hour));
            if (timerPeriod.Minute > 0) result.Append(GetFormat(timerPeriod.Minute));
            if (timerPeriod.Second >= 0) result.Append(GetFormat(timerPeriod.Second));

            return result.ToString();
        }

        public static string GetFormat(int time)
        {
            if (time < 10)
                return time.ToString("0");
            else return time.ToString("00");
        }

        public static DateTime CalculateTimePeriod(long waitingTime, long ticks)
        {
            DateTime period = new DateTime();

            if (waitingTime >= ticks)
                period = DateTime.FromBinary(waitingTime - ticks);

            return period;
        }
    }
}
