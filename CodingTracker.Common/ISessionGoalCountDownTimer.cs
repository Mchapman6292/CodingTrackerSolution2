using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodingTracker.Common.ISessionGoalCountDownTimer
{
    public interface ISessionGoalCountDownTimer
    {
        void SetCountDownTimer(int minutes, int seconds = 0);
        void StartCountDownTimer();

        void CheckTimeCountDownTimer();

        void StopCountDownTimer();
        void Reset();

        void Dispose();

        void Timer_Tick(object sender, EventArgs e);

        event Action<TimeSpan> TimeChanged;
        event Action CountDownFinished;

    }
}
