using System;
using System.Threading;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public abstract class CyclicCacheGenerator
    {
        private CancellationTokenSource _cancellationTokenSource;

        public async void StartGeneratingStatsCacheEveryday()
        {
            var isNowRunning = _cancellationTokenSource?.IsCancellationRequested == false;
            if (isNowRunning)
            {
                return;
            }
            _cancellationTokenSource = new CancellationTokenSource();
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                await BlockUntilNextNight();
                await ReloadCache();
            }
        }

        public void StopGeneratingStatsCacheEveryday()
        {
            _cancellationTokenSource.Cancel();
        }

        protected abstract Task ReloadCache();

        private async Task BlockUntilNextNight()
        {
            const int hourWhenShouldGenerateCyclicStatistics = 02; // 24h clock

            var nightTimeThisDay = DateTime.Today.AddHours(hourWhenShouldGenerateCyclicStatistics); // always 2:00AM this day
            var nextNight = DateTime.Now.Hour < hourWhenShouldGenerateCyclicStatistics
                ? nightTimeThisDay
                : nightTimeThisDay.AddDays(1);

            var delay = nextNight - DateTime.Now;
            await Task.Delay(delay, _cancellationTokenSource.Token);
        }
    }
}
