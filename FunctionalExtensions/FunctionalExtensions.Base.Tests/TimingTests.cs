using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalExtensions.Base.Tests
{
    public class TimingTests
    {
        [Fact]
        public void TestBlockingDataResultTimeout()
        {
            Func<int> shortOp = () => { Task.Delay(50).Wait(); return 50; };
            Func<int> longOp = () => { Task.Delay(1500).Wait(); return 1500; };

            var shortOpResult = shortOp.RunWithTimeout(700);
            var longOpResult = longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);
            Assert.True(shortOpResult.HasData);
            Assert.Equal(50, shortOpResult.Data);

            Assert.True(longOpResult.IsFailure);
        }

        [Fact]
        public async void TestAsyncOperationTimeout()
        {
            Func<Task<int>> shortOp = async () => { await Task.Delay(50); return 50; };
            Func<Task<int>> longOp = async () => { await Task.Delay(1500); return 1500; };

            var shortOpResult = await shortOp.RunWithTimeout(700);
            var longOpResult = await longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);
            Assert.True(shortOpResult.HasData);
            Assert.Equal(50, shortOpResult.Data);

            Assert.True(longOpResult.IsFailure);
        }

        [Fact]
        public async void TestDataResultTimeout()
        {
            Func<DataResult<int>> shortOp = () => ResultExtensions.AsDataResult(() =>
            {
                Task.Delay(50).Wait(); 
                return 50;
            });
            Func<DataResult<int>> longOp = () => ResultExtensions.AsDataResult(() =>
            {
                Task.Delay(1500).Wait();
                return 50;
            });

            var shortOpResult = await shortOp.RunWithTimeout(700);
            var longOpResult = await longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);
            Assert.True(shortOpResult.HasData);
            Assert.Equal(50, shortOpResult.Data);

            Assert.True(longOpResult.IsFailure);
        }

        [Fact]
        public async void TestAsyncOperationDataResultTimeout()
        {
            var shortOp = async () => await ResultExtensions.AsDataResult(
                async () =>
            {
                await Task.Delay(50);
                return 50;
            });
            var longOp = async () => await ResultExtensions.AsDataResult(
                async () =>
            {
                await Task.Delay(1500);
                return 50;
            });

            var shortOpResult = await shortOp.RunWithTimeout(700);
            var longOpResult = await longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);
            Assert.True(shortOpResult.HasData);
            Assert.Equal(50, shortOpResult.Data);

            Assert.True(longOpResult.IsFailure);
        }


        [Fact]
        public void TestOperationBoolResultTimeout()
        {
            var shortOp = () =>
                {
                    Task.Delay(50).Wait();
                    return true;
                };
            var longOp = () =>
                {
                    Task.Delay(1500).Wait();
                    return true;
                };

            var shortOpResult = shortOp.RunWithTimeout(700);
            var longOpResult = longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);

            Assert.True(longOpResult.IsFailure);
        }

        [Fact]
        public async void TestAsyncOperationBoolResultTimeout()
        {
            var shortOp = async () =>
            {
                await Task.Delay(50);
                return true;
            };
            var longOp = async () =>
            {
                await Task.Delay(1500);
                return true;
            };

            var shortOpResult = await shortOp.RunWithTimeout(700);
            var longOpResult = await longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);

            Assert.True(longOpResult.IsFailure);
        }

        [Fact]
        public async void TestOperationResultTimeout()
        {
            var shortOp = () => ResultExtensions.AsResult(
                () =>
                {
                    Task.Delay(50).Wait();
                    return true;
                });

            var longOp = () => ResultExtensions.AsResult(
                () =>
                {
                    Task.Delay(1500).Wait();
                    return true;
                });

            var shortOpResult = await shortOp.RunWithTimeout(700);
            var longOpResult = await longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);

            Assert.True(longOpResult.IsFailure);
        }

        [Fact]
        public async void TestAsyncOperationResultTimeout()
        {
            var shortOp = async () => await ResultExtensions.AsResult(
                async () =>
                {
                    await Task.Delay(50);
                    return true;
                });

            var longOp = async () => await ResultExtensions.AsResult(
                async () =>
                {
                    await Task.Delay(1500);
                    return true;
                });

            var shortOpResult = await shortOp.RunWithTimeout(700);
            var longOpResult = await longOp.RunWithTimeout(700);

            Assert.True(shortOpResult.IsSuccess);

            Assert.True(longOpResult.IsFailure);
        }

        [Fact]
        public void WhileTrueTest()
        {
            var whileTrueOp = new Func<int>(() => { while (true) { System.Diagnostics.Trace.WriteLine($"doing op at:{DateTime.Now.Ticks}"); } return 64; });

            var result = whileTrueOp.RunWithTimeout(1000);

            Assert.True(result.IsFailure);
            System.Diagnostics.Trace.WriteLine($"finished test at:{DateTime.Now.Ticks}");
            Assert.False(result.IsSuccess);
            System.Diagnostics.Trace.WriteLine($"finished test at:{DateTime.Now.Ticks}");
            System.Diagnostics.Trace.WriteLine($"finished test at:{DateTime.Now.Ticks}");
            System.Diagnostics.Trace.WriteLine($"finished test at:{DateTime.Now.Ticks}");

        }
    }
}
