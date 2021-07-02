using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionalExtensions.Base.Tests
{
    public class DisposingTests
    {
        [Fact]
        public void UsingDisposedCorrectlyTest()
        {
            MockDisposable mockDisposable = null;
            var result = 
                Disposing.Using(() => { mockDisposable = new MockDisposable(); return mockDisposable; },
                                disp => disp.RunOperation());

            Assert.NotNull(mockDisposable);
            Assert.True(mockDisposable.HasRunOperation);
            Assert.True(mockDisposable.IsDisposed);

        }


        private class MockDisposable : IDisposable
        {
            public bool IsDisposed { get; private set; }
            public bool HasRunOperation { get; private set; }

            public MockDisposable()
            {
                IsDisposed = false;
                HasRunOperation = false;
            }

            public int RunOperation()
            {
                HasRunOperation = true;
                return 1;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
