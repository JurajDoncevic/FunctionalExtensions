using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using FunctionalExtensions.Base;
using System.Threading.Tasks;

namespace FunctionalExtensions.Base.Tests
{
    public class AwaitingTests
    {
        [Fact]
        public void AwaitAnAsyncTaskTest()
        {
            int result =
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    return 1;
                }).WaitFor(r => r + 1);

            Assert.Equal(2, result);
        }

        [Fact]
        public void AwaitTaskUnderTimeoutTest()
        {
            int result =
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    return 1;
                }).WaitFor(r => r + 1, 150);

            Assert.Equal(2, result);
        }

        [Fact]
        public void AwaitTaskOverTimeoutTest()
        {
            int result =
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    return 1;
                }).WaitFor(r => r + 1, 50);

            Assert.Equal(default(int), result);
        }
    }
}
