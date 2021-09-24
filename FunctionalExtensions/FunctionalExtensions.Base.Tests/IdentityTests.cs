using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FunctionalExtensions.Base.Tests
{
    public class IdentityTests
    {
        [Fact]
        public void IdentityReturnTest()
        {
            var identity1 = 2.Identity();
            var identity2 = (X: 2, Y: 3).Identity();

            Assert.Equal(2, identity1.Data);
            Assert.Equal((X: 2, Y: 3), identity2.Data);
        }

        [Fact]
        public void IdentityMapValueTypeTest()
        {
            var valueTuple = (X: 2, Y: 3, Z: 4);

            var result =
                valueTuple.Identity()
                          .Map(_ => (_.X, _.Y))
                          .Map(_ => _.X + 1)
                          .Data;

            Assert.Equal(3, result);
        }

        [Fact]
        public void IdentityMapReferenceTypeTest()
        {
            var obj = new TestObj() { X = 2, Y = 3 };

            var result =
                obj.Identity()
                   .Map(_ => new TestObj { X = ++_.X, Y = _.Y })
                   .Data;

            Assert.Equal(3, result.X);
            Assert.Equal(3, obj.X);
        }


        private class TestObj
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

    }
}
