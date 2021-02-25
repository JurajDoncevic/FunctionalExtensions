using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.Base
{
    public static class Piping
    {
        public static T Pass<T>(this T target, Func<T, Unit> func)
        {
            func(target);
            return target;
        }
    }
}
