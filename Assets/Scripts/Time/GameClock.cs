using System;
using System.Collections.Generic;

public sealed class GameClock
{
    public const double RealSecondsPerDay=300;
    public const double GameSecondsPerDay=86400;
    private readonly List<IGameTimeListener> _listeners=new List<IGameTimeListener>();
    private double _secondsOfDay=7*3600;
    public int Day { get; private set; }=1;
    public int Hour => (int)(_secondsOfDay/3600);
    public int Minute => (int)(_secondsOfDay/60)%60;
    public double MinuteOfDay => _secondsOfDay/60;
    public void Subscribe(IGameTimeListener listener)
    { if(listener==null)throw new ArgumentNullException(nameof(listener));if(!_listeners.Contains(listener))_listeners.Add(listener); }
    public void Unsubscribe(IGameTimeListener listener) { _listeners.Remove(listener); }
    public void Advance(double realSeconds)
    {
        if(double.IsNaN(realSeconds) || double.IsInfinity(realSeconds) || realSeconds<0)
            throw new ArgumentOutOfRangeException(nameof(realSeconds));
        double remaining=realSeconds*(GameSecondsPerDay/RealSecondsPerDay);
        if(double.IsInfinity(remaining) || remaining>(int.MaxValue-(double)Day)*GameSecondsPerDay)
            throw new ArgumentOutOfRangeException(nameof(realSeconds));
        // Deliver every elapsed hour/day in order, including when one frame spans midnight.
        while(remaining>0)
        {
            double nextHour=(Hour+1)*3600;
            double step=Math.Min(remaining,nextHour-_secondsOfDay);
            bool boundary=step>=nextHour-_secondsOfDay;
            _secondsOfDay+=step;remaining-=step;
            foreach(var listener in _listeners.ToArray())listener.OnTimeElapsed((float)step);
            if(!boundary)continue;
            _secondsOfDay=nextHour;
            if(_secondsOfDay>=GameSecondsPerDay)
            {
                _secondsOfDay=0;Day++;
                foreach(var listener in _listeners.ToArray())listener.OnDayChanged(Day);
            }
            foreach(var listener in _listeners.ToArray())listener.OnHourChanged(Day,Hour);
        }
    }
    public static string FormatDuration(double gameSeconds)
    {
        if(double.IsNaN(gameSeconds) || double.IsInfinity(gameSeconds) || gameSeconds<0)
            throw new ArgumentOutOfRangeException(nameof(gameSeconds));
        long total=(long)Math.Ceiling(gameSeconds/60);
        long days=total/1440,hours=total/60%24,minutes=total%60;
        string text=days>0 ? days+" ngày" : "";
        if(hours>0)text+=(text.Length>0 ? " " : "")+hours+" giờ";
        if(minutes>0 || text.Length==0)text+=(text.Length>0 ? " " : "")+minutes+" phút";
        return text;
    }
}
