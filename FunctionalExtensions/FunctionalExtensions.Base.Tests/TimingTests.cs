using FunctionalExtensions.Base.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalExtensions.Base.Tests
{
    public class TimingTests
    {
        [Fact, Trait("Category", "TimingTest")]
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

        [Fact, Trait("Category", "TimingTest")]
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

        [Fact, Trait("Category", "TimingTest")]
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

        [Fact, Trait("Category", "TimingTest")]
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


        [Fact, Trait("Category", "TimingTest")]
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

        [Fact, Trait("Category", "TimingTest")]
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

        [Fact, Trait("Category", "TimingTest")]
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

        [Fact, Trait("Category", "TimingTest")]
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

        [Fact, Trait("Category", "TimingTest")]
        public void BlockingDataResultTest()
        {
            var operation = (CancellationToken token) =>
            {
                Task.Delay(20).Wait();
                token.ThrowIfCancellationRequested();
                return 64;
            };

            var result = operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
            Assert.True(result.HasData);
            Assert.Equal(64, result.Data);

        }

        [Fact, Trait("Category", "TimingTest")]
        public void BlockingDataResultWhileTrueTest()
        {
            var whileTrueOp = new Func<CancellationToken, int>(token => { while (true) { token.ThrowIfCancellationRequested(); } return 64; });

            var result = whileTrueOp.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);

        }

        [Fact, Trait("Category", "TimingTest")]
        public void BlockingDataResultWhileTrueThrowsExceptionTest()
        {
            var whileTrueOp = new Func<CancellationToken, int>(token => { while (true) { token.ThrowIfCancellationRequested(); throw new Exception("TEST"); } return 64; });

            var result = whileTrueOp.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);

        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationTest()
        {
            Func<CancellationToken, Task<int>> operation =
                async token =>
                {
                    await Task.Delay(1);
                    token.ThrowIfCancellationRequested();
                    return 700;
                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
            Assert.Equal(700, result.Data);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueTest()
        {
            Func<CancellationToken, Task<int>> operation =
                async token =>
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                    }
                    return 700;
                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueThrowsExceptionTest()
        {
            Func<CancellationToken, Task<int>> operation =
                async token =>
                {
                    while (true)
                    {
                        throw new Exception("TEST");
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);

                    }
                    return 700;
                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncDataResultOperationTest()
        {
            Func<CancellationToken, Task<DataResult<int>>> operation =
                async token =>
                {
                    return await ResultExtensions.AsDataResult(async () =>
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
            Assert.Equal(700, result.Data);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncDataResultOperationWhileTrueTest()
        {
            Func<CancellationToken, Task<DataResult<int>>> operation =
                async token =>
                {
                    return await ResultExtensions.AsDataResult(async () =>
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            await Task.Delay(1);
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncDataResultOperationWhileTrueThrowsExceptionTest()
        {
            Func<CancellationToken, Task<DataResult<int>>> operation =
                async token =>
                {
                    return await ResultExtensions.AsDataResult(async () =>
                    {
                        while (true)
                        {
                            throw new Exception("TEST");
                            token.ThrowIfCancellationRequested();
                            await Task.Delay(1);
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void DataResultOperationTest()
        {
            Func<CancellationToken, DataResult<int>> operation =
                token =>
                {
                    return ResultExtensions.AsDataResult(() =>
                    {
                        token.ThrowIfCancellationRequested();
                        Task.Delay(1).Wait();
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
            Assert.Equal(700, result.Data);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void DataResultOperationWhileTrueTest()
        {
            Func<CancellationToken, DataResult<int>> operation =
                token =>
                {
                    return ResultExtensions.AsDataResult(() =>
                    {
                        while (true)
                        {
                            token.ThrowIfCancellationRequested();
                            Task.Delay(1).Wait();
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void DataResultOperationWhileTrueThrowsExceptionTest()
        {
            Func<CancellationToken, DataResult<int>> operation =
                token =>
                {
                    return ResultExtensions.AsDataResult(() =>
                    {
                        while (true)
                        {
                            throw new Exception("TEST");
                            token.ThrowIfCancellationRequested();
                            Task.Delay(1).Wait();
                        }
                        return 700;
                    });


                };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsFailure);
            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public void OperationBoolResultTest()
        {
            var operation = (CancellationToken token) =>
            {
                token.ThrowIfCancellationRequested();
                Task.Delay(1).Wait();
                return true;
            };

            var result = operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public void OperationBoolWhileTrueResultTest()
        {
            var operation = (CancellationToken token) =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    Task.Delay(1).Wait();
                }
                return true;
            };

            var result = operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public void OperationBoolWhileTrueResultExceptionTest()
        {
            var operation = (CancellationToken token) =>
            {
                while (true)
                {
                    throw new Exception("TEST");
                    token.ThrowIfCancellationRequested();
                    Task.Delay(1).Wait();
                }
                return true;
            };

            var result = operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationBoolResultTest()
        {
            var operation = async (CancellationToken token) =>
            {
                token.ThrowIfCancellationRequested();
                await Task.Delay(1);

                return true;
            };

            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationBoolWhileTrueResultTest()
        {
            var operation = async (CancellationToken token) =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1);
                }
                return true;
            };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationBoolWhileTrueResultExceptionTest()
        {
            var operation = async (CancellationToken token) =>
            {
                while (true)
                {
                    throw new Exception("TEST");
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1);
                }
                return true;
            };

            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void OperationResultTimeoutTest()
        {
            var operation = (CancellationToken token) => ResultExtensions.AsResult(
                () =>
                {
                    Task.Delay(1).Wait();
                    token.ThrowIfCancellationRequested();
                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void OperationResultWhileTrueTimeoutTest()
        {
            var operation = (CancellationToken token) => ResultExtensions.AsResult(
                () =>
                {
                    while (true)
                    {
                        Task.Delay(1).Wait();
                        token.ThrowIfCancellationRequested();
                    }
                    return true;
                });


            var result = await operation.RunWithTimeout(700);
            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);

        }

        [Fact, Trait("Category", "TimingTest")]
        public async void OperationResultWhileTrueExceptionTest()
        {
            var operation = (CancellationToken token) => ResultExtensions.AsResult(
                () =>
                {
                    while (true)
                    {
                        throw new Exception("TEST");
                        Task.Delay(1).Wait();
                        token.ThrowIfCancellationRequested();
                    }
                    return true;
                });


            var result = await operation.RunWithTimeout(700);
            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationResultTimeoutTest()
        {
            var operation = async (CancellationToken token) => await ResultExtensions.AsResult(
                async () =>
                {
                    token.ThrowIfCancellationRequested();
                    await Task.Delay(1);
                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.True(result.IsSuccess);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueResultTimeoutTest()
        {
            var operation = async (CancellationToken token) => await ResultExtensions.AsResult(
                async () =>
                {
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                    }

                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal(Timing.TIMEOUT_ERROR_MESSAGE, result.ErrorMessage);
        }

        [Fact, Trait("Category", "TimingTest")]
        public async void AsyncOperationWhileTrueThrowsExceptionResultTimeoutTest()
        {
            var operation = async (CancellationToken token) => await ResultExtensions.AsResult(
                async () =>
                {
                    while (true)
                    {
                        throw new Exception("TEST");
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(1);
                    }

                    return true;
                });


            var result = await operation.RunWithTimeout(700);

            Assert.False(result.IsSuccess);
            Assert.Equal("TEST", result.ErrorMessage);
        }



    }
}
