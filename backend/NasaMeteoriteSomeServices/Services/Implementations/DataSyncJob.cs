using Quartz;
using NasaMeteoriteSomeServices.Services.Interfaces;

namespace NasaMeteoriteSomeServices.Services.Implementations
{
    public class DataSyncJob : IJob
    {
        private readonly IMeteoriteSyncService _syncService;

        public DataSyncJob(IMeteoriteSyncService syncService)
        {
            _syncService = syncService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _syncService.SyncMeteoriteDataAsync();
        } 
    }
}
