public interface IGameTimeListener
{
    void OnTimeElapsed(float deltaTime);
    void OnHourChanged(int day,int hour);
    void OnDayChanged(int day);
}
