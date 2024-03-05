using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ResourceHealthChecker;
using SlugEnt.APIInfo.HealthInfo;
using SlugEnt.ResourceHealthChecker;

namespace SlugEnt.APIInfo.Sample
{
    /// <summary>
    /// Performs backgroun API Health Checks
    /// </summary>
    public class APIBackgroundProcessor : BackgroundService
    {
        private ILogger<APIBackgroundProcessor> _logger;
        private IServiceProvider                _serviceProvider;
        private HealthCheckProcessor            _healthCheckProcessor;
        private EnumHealthStatus                _lastHealthStatus = EnumHealthStatus.Unknown;
        private int                             _lastStatusCount  = 0;


        /// <summary>
        /// Constructs the API Background Processor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public APIBackgroundProcessor(ILogger<APIBackgroundProcessor> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _logger               = logger;
            _serviceProvider      = serviceProvider;
            _healthCheckProcessor = _serviceProvider.GetService<HealthCheckProcessor>();
        }



        /// <summary>
        /// Main Processing loop for background health checks
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(StopService);

            TimeSpan sleepTime = TimeSpan.FromSeconds(5);
            while (!stoppingToken.IsCancellationRequested)
            {
                _healthCheckProcessor.CheckHealth();

                // We sleep for a second to allow health checks to run then check the status.  Note, some may still be running, especially if they are erroring, there status will be updated next run.
                await Task.Delay(1000, stoppingToken);

                EnumHealthCheckProcessorStage healthStage = _healthCheckProcessor.ProcessingStage;
                if (healthStage != EnumHealthCheckProcessorStage.Processing)
                {
                    if (healthStage == EnumHealthCheckProcessorStage.FailedToStart)
                        StopAsync(stoppingToken);
                }

                EnumHealthStatus currentStatus = _healthCheckProcessor.Status;

                if (currentStatus != _lastHealthStatus)
                {
                    string msg = "  It was previously in a " + _lastHealthStatus.ToString() + " state for " + _lastStatusCount + " Health Cycle Checks.";
                    if (currentStatus == EnumHealthStatus.Healthy)
                        _logger.LogWarning("Health Status has returned to a Healthy State." + msg);
                    else if (currentStatus == EnumHealthStatus.Failed)
                        _logger.LogCritical("Health status has changed to a Failed State.  The service will likely not operate correctly." + msg);
                    else if (currentStatus == EnumHealthStatus.Degraded)
                        _logger.LogError("Health Status has changed to a Degraded State.  This may or may not have impacts on the service.  Investigation should immediately be looked into." +
                                         msg);
                    else if (currentStatus == EnumHealthStatus.Unknown)
                        _logger.LogError("Health Status is unknown.  This should be short term upon initial application start.  If it does not change shortly, then something is wrong.");

                    _lastHealthStatus = currentStatus;
                    _lastStatusCount  = 1;
                }

                // Sleep for cycle time.
                //await Task.Delay(sleepTime, stoppingToken);
            }
        }


        private void StopService() { }


        /// <summary>
        /// The amount of time between checks the background processor should sleep for.  This should typically be a short period of time.  This is just the amount of time between a check cycle which
        /// means the time it should sleep before seeing IF a health check needs to be checked.  Individual health checks may have much longer cycles.  This is typically under 2 minutes.
        /// </summary>
        public int SleepIntervalSeconds { get; private set; } = 30;
    }
}