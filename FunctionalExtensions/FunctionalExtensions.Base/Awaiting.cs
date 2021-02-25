using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base
{
    public static class Awaiting
    {
        /// <summary>
        /// Extension method for awating a task result and operating over it.
        /// Uses Task.Result call - not to be used lightly!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="task"></param>
        /// <param name="operation"></param>
        /// <returns>Sync result of operation</returns>
        public static R WaitFor<T, R>(this Task<T> task, Func<T, R> operation)
            => operation(task.Result);
    }
}
