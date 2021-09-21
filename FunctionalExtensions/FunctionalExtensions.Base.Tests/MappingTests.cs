using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FunctionalExtensions.Base.Tests
{
    public class MappingTests
    {
        [Fact]
        public void MapOnObjectTest()
        {
            object aObject = new { x = "X", y = "Y" };

            var result = aObject.MapSingle(_ => _.ToString());

            Assert.Equal(aObject.ToString(), result);
        }

        [Fact]
        public void MapOnPrimitiveTest()
        {
            int aInt = 1;

            var result = aInt.MapSingle(_ => _ + 2);

            Assert.Equal(3, result);
        }

        [Fact]
        public void MapOnValueTuple()
        {
            var tuple = ("1", 2);

            var result = tuple.MapSingle(_ => (_.Item2, _.Item1))
                              .MapSingle(_ => (_.Item2, _.Item1));

            Assert.Equal(tuple, result);
        }

        [Fact]
        public void MapExplicitlyOnValueTuple()
        {
            var tuple = (x: "1", y: 2);

            var result = tuple.MapSingle(_ => (a: _.y, b: _.x))
                              .MapSingle(_ => (x: _.b, y: _.a));

            Assert.Equal(tuple, result);
        }

        [Fact]
        public void MapOverList()
        {
            var list = Enumerable
                        .Range(1, 10);

            var expectedList = list.Select(_ => _ + 1).ToList();

            var mappedList = list.Map(_ => _ + 1);

            Assert.Equal(expectedList, mappedList);
        }

        [Fact]
        public void MapiOverList()
        {
            var list = Enumerable
                        .Range(0, 10);

            var expectedList = list.Select(_ => _ * 2).ToList();

            var mappedList = list.Mapi((idx, element) => (int)idx + element);

            Assert.Equal(expectedList, mappedList);
        }

        [Fact]
        public async void MapOverTask()
        {
            var result = Task.Run(() => { System.Threading.Thread.Sleep(100); return 1; })
                           .Map(_ => _ + 1)
                           .Map(_ => _ + 1);
                           
            Assert.Equal(3, await result);
        }
    }
}
