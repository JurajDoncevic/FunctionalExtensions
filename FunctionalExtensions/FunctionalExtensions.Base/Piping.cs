using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.Base
{
    public static class Piping
    {
        /// <summary>
        /// Executes a Unit returning/side-effect function wiht target as param and passes the target onward.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Side-effect function</param>
        /// <returns></returns>
        public static T Pass<T>(this T target, Func<T, Unit> func)
        {
            func(target);
            return target;
        }
    }
}
