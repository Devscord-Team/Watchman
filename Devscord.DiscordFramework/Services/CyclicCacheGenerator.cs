using System;
using System.Threading;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public abstract class CyclicCacheGenerator
    {
        private CancellationTokenSource _cancellationTokenSource;

        public async void StartCyclicCaching()
        {
            var isNowRunning = this._cancellationTokenSource?.IsCancellationRequested == false;
            if (isNowRunning)
            {
                return;
            }
            this._cancellationTokenSource = new CancellationTokenSource();
            while (!this._cancellationTokenSource.IsCancellationRequested)
            {
                await BlockUntilNextNight();
                await ReloadCache();
            }
        }

        public void StopCyclicCaching()
        {
            this._cancellationTokenSource.Cancel();
        }

        protected abstract Task ReloadCache();

        private async Task BlockUntilNextNight()
        {
            const int hourWhenShouldGenerateCyclicStatistics = 2; // 24h clock

            var nightTimeThisDay = DateTime.Today.AddHours(hourWhenShouldGenerateCyclicStatistics); // always 2:00AM this day
            var nextNight = DateTime.Now.Hour < hourWhenShouldGenerateCyclicStatistics
                ? nightTimeThisDay
                : nightTimeThisDay.AddDays(1);

            var delay = nextNight - DateTime.Now;
            await Task.Delay(delay, this._cancellationTokenSource.Token);
        }
    }
}
