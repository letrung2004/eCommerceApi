using InventoryService.Application.Jobs;
using Quartz;

namespace InventoryService.Presentation.Configuration
{
    public static class QuartzConfig
    {
        public static void AddQuartzJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                var jobKey = new JobKey("ExpiredReservationCleanupJob");

                q.AddJob<ExpiredReservationCleanupJob>(opts => opts.WithIdentity(jobKey));

                q.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity("ExpiredReservationCleanupTrigger")
                    .WithSimpleSchedule(x => x
                        .WithInterval(TimeSpan.FromMinutes(5))
                        .RepeatForever()));
            });

            services.AddQuartzHostedService();
        }
    }
}
