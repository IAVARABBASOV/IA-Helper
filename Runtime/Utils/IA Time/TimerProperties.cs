using System;
using UnityEngine;

namespace IA.Utils
{
    [System.Serializable]
    public class TimerProperties
    {
        public bool HasData;

        [SerializeField, Header("Milli Sec"), Range(0, 999)] private int milliSec = 0;
        public int GetMilliSecond => milliSec;

        [SerializeField, Header("Sec"), Range(0, 59)] private int second = 5;
        public int GetSecond => second;

        [SerializeField, Header("Min"), Range(0, 59)] private int minute = 0;
        public int GetMinute => minute;

        [SerializeField, Header("Hour"), Range(0, 23)] private int hour = 0;
        public int GetHour => hour;

        [SerializeField] private bool GetAutoDate = true;
        public void SetAutoDate(bool enable) => GetAutoDate = enable;

        [SerializeField, Header("Day")] private int day = 0;
        public int GetDay => day;

        [SerializeField, Header("Month")] private int month = 0;
        public int GetMonth => month;

        [SerializeField, Header("Year")] private int year = 0;
        public int GetYear => year;

        public TimerProperties(bool hasData = true)
        {
            HasData = hasData;
        }

        public TimerProperties SetTimer(DateTime time)
        {
            this.year = GetAutoDate ? DateTime.Now.Year : time.Year;
            this.month = GetAutoDate ? DateTime.Now.Month : time.Month;
            this.day = GetAutoDate ? DateTime.Now.Day : time.Day;

            this.hour = time.Hour;
            this.minute = time.Minute;
            this.second = time.Second;
            this.milliSec = time.Millisecond;

            return this;
        }

        public TimerProperties SetTimer(int year,int month, int day,int hour, int minute, int second, int millisecond)
        {
            this.year = year;
            this.month = month;
            this.day = day;
            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.milliSec = millisecond;

            return this;
        }

        public TimerProperties SetTimer(int hour, int minute, int second, int millisecond)
        {
            GetAutoDate = true;

            this.year = GetAutoDate ? DateTime.Now.Year : year;
            this.month = GetAutoDate ? DateTime.Now.Month : month;
            this.day = GetAutoDate ? DateTime.Now.Day : day;

            this.hour = hour;
            this.minute = minute;
            this.second = second;
            this.milliSec = millisecond;

            return this;
        }

        public bool Compare(TimerProperties _target)
        {
            return 
                _target.GetYear == GetYear &&
                _target.GetMonth == GetMonth &&
                _target.GetDay == GetDay &&
                _target.GetHour == GetHour &&
                _target.GetMinute == GetMinute &&
                _target.GetSecond == GetSecond &&
                _target.GetMilliSecond == GetMilliSecond;
        }

        public DateTime GetAsDateTime()
        {
            return new DateTime(year: year, month: month, day: day, hour: hour, minute: minute, second: second, millisecond: milliSec);
        }
    }
}
