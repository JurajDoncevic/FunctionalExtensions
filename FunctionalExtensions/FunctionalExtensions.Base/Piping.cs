using FunctionalExtensions.Base;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable
namespace FunctionalExtensions.Base
{
    public static class Piping
    {
        /// <summary>
        /// Executes a Unit returning/side-effect function with target as the parameter and passes the target onward in the pipeline.
        /// Side effects preserved if done on a mutable-type target (class)! 
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


        /// <summary>
        /// Passes argument to function if it is not equal to null. Turns classes into Monads.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R> func)
            where T : class
            where R : class
            => target != null
                ? (R?)func(target)
                : null;

        /// <summary>
        /// Passes argument to function if it is not equal to null. Turns structs into Monads.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R?> func)
            where T : struct
            where R : struct
            => target.HasValue
                ? (R?)func(target.Value)
                : null;

        /// <summary>
        /// Passes argument to function if it is not equal to null. Natural transformation between classes and structs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R?> func)
            where T : class
            where R : struct
            => target != null
                ? (R?)func(target)
                : null;

        /// <summary>
        /// Passes argument to function if it is not equal to null. Natural transformation between classes and structs.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="target">Target object</param>
        /// <param name="func">Operation function</param>
        /// <returns></returns>
        public static R? IfNotNull<T, R>(this T? target, Func<T, R?> func)
            where T : struct
            where R : class
            => target.HasValue
                ? (R?)func(target.Value)
                : null;

    }
}

