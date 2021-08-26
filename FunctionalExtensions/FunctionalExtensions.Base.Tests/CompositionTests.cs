using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionalExtensions.Base.Tests
{
    
    public class CompositionTests
    {

        [Fact]
        public void OneParamFuncCompositionTest()
        {
            Func<int, int> inc5 = Increment5;

            var operation =
                inc5.Compose(Decrement2)
                    .Compose(Decrement2)
                    .Compose(Decrement2)
                    .Compose(IsNegative);

            Assert.True(operation(0));
            Assert.False(operation(1));
        }

        [Fact]
        public void MultiParamFuncCompositionTest()
        {
            Func<int, int, int, int> sum3 = Sum3;
            Functional x = Sum3;
            var operation =
                sum3.Compose(Increment5)
                    .Compose(((Func<int, int, int>)(Sub)).Apply(5))
                    .Compose(IsPositive);

            Assert.True(operation(0, 0, 0));
            Assert.False(operation(1, 0, 0));
        }


        #region TEST METHODS
        private int Increment5(int x) => x + 5;

        private int Decrement2(int x) => x - 2;

        private bool IsNegative(int x) => x < 0;

        private bool IsPositive(int x) => x >= 0;

        private int Add(int x, int y) => x + y;

        private int Sub(int x, int y) => x - y;

        private int Sum3(int x, int y, int z) => x + y + z;
        #endregion
    }
}
