using Common.Utilities;
using Hangfire;
using Hangfire.Client;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces.Services
{
    public class BackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public BackgroundJobService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }
      
        // do not wait for proceeding task and goes on before the task completed
        public void EnqueueFireAndForget(Func<Task> task)
        {
            _backgroundJobClient.Enqueue(() => task.Invoke());
        }

        // schedule a task to executed by cron Expression  (executed on a specific time)
        public void ScheduleRecurring(Func<Task> task, string cronExpression)
        {
            RecurringJob.AddOrUpdate(() => task.Invoke(), cronExpression);
        }

        // schedule a task to executed (delay for execution)
        public void ScheduleDelayed(Func<Task> task, TimeSpan delay)
        {
            _backgroundJobClient.Schedule(() => task.Invoke(), delay);
        }

        // queue a task for execution and waits for task to be completed
        public async Task EnqueueTask(Func<Task> task)
        {
            await task.Invoke();
        } 
    }
    }
