using FunctionalExtensions.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FunctionalExtensions.Tests
{
    public class BaseTests
    {
        [Fact]
        public static void FoldOnSameTypesTest()
        {
            const string testCsv = "This;is;some;rediculous;Csv;line";

            var csvSplit = testCsv.Split(';').ToList();

            string finalCsv = csvSplit.Skip(1).Fold(csvSplit.First(), (item, seed) => seed + ";" + item);

            Assert.NotNull(finalCsv);
            Assert.NotEmpty(finalCsv);
            Assert.Equal(testCsv, finalCsv);
        }

        [Fact]
        public static void FoldOnDifferentTypesTest()
        {
            const string testString = "12345";

            List<int> testInts = new List<int>{ 1, 2, 3, 4, 5 };

            string finalCsv = testInts.Skip(1).Fold(testInts.First().ToString(), (item, seed) => seed + item.ToString());

            Assert.NotNull(finalCsv);
            Assert.NotEmpty(finalCsv);
            Assert.Equal(testString, finalCsv);
        }
    }
}
