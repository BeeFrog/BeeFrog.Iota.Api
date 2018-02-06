using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BeeFrog.Iota.Api
{
    public static class TaskIota
    {
        /// <summary>
        /// For async functions. Can be used like TaskIota.Yield().ConfigureAwait(false);
        /// </summary>
        /// <returns></returns>
        public static async Task Yield()
        {
            await Task.Yield();
        }
    }
}
