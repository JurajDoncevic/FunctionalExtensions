using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalExtensions.Base
{
    public static class Composition
    {
        public static Func<T1, R> Compose<T1, T2, R>(this Func<T1, T2> before, Func<T2, R> after)
            => _ => after(before(_));

        public static Func<T1, T2, R> Compose<T1, T2, T3, R>(this Func<T1, T2, T3> before, Func<T3, R> after)
            => (_1, _2) => after(before(_1, _2));

        public static Func<T1, T2, T3, R> Compose<T1, T2, T3, T4, R>(this Func<T1, T2, T3, T4> before, Func<T4, R> after)
            => (_1, _2, _3) => after(before(_1, _2, _3));

        public static Func<T1, T2, T3, T4, R> Compose<T1, T2, T3, T4, T5, R>(this Func<T1, T2, T3, T4, T5> before, Func<T5, R> after)
            => (_1, _2, _3, _4) => after(before(_1, _2, _3, _4));

        public static Func<T1, T2, T3, T4, T5, R> Compose<T1, T2, T3, T4, T5, T6, R>(this Func<T1, T2, T3, T4, T5, T6> before, Func<T6, R> after)
            => (_1, _2, _3, _4, _5) => after(before(_1, _2, _3, _4, _5));

    }
}
