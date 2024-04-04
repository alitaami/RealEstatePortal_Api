using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBackgroundJobService
    {
        // Enqueue a fire-and-forget task
        void EnqueueFireAndForget(Func<Task> task);

        // Schedule a recurring task
        void ScheduleRecurring(Func<Task> task, string cronExpression);

        // Schedule a delayed task
        void ScheduleDelayed(Func<Task> task, TimeSpan delay);

        // Enqueue a task to be processed immediately
        Task EnqueueTask(Func<Task> task);
     }
}
