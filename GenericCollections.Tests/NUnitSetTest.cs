using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GenericCollections.Tests
{
    [TestFixture]
    public class NUnitSetTest
    {
        [Test]
        public void TestAdd()
        {
            var set = new Set<int>(new[] { 1, 2, 3, 4, 5 });
            set.Add(632);
            set.Add(-766);
            set.Add(1);

            Assert.IsTrue(EqualSet(set, new Set<int> { 1, 2, 3, 4, 5, 632, -766 }));
        }

        [Test]
        public void TestContains()
        {
            int[] expectedResult = { 535, 10, 2, 3, 666,1, -2 };
            Set<int> set = new Set<int>(expectedResult);

            Assert.IsFalse(set.Contains(-288));
            Assert.IsTrue(set.Contains(-2));
            Assert.IsFalse(set.Contains(55));
            Assert.IsTrue(set.Contains(666));
        }

        [Test]
        public void TestStaticUnion()
        {
            int[] firstArr = { 1, 2, 3, 4, 5 };
            int[] secondArr = { 3, 4, 5, 6, 7,77 };
            Set<int> firstSet = new Set<int>(firstArr);
            Set<int> secondSet = new Set<int>(secondArr);
            Set<int> result = Set<int>.Union(firstSet, secondSet);
            
            Assert.IsTrue(EqualSet(result, new Set<int>(new[] { 1, 2, 3, 4, 5, 6, 7 ,77})));
        }

        [Test]
        public void TestRemove()
        {
            var set = new Set<int>(new[] { 2, 3, 4, 5, 6, 7 });
            var resultSet = new Set<int>(new int[] { });
            set.Remove(2);
            set.Remove(3);
            set.Remove(4);
            set.Remove(6);
            set.Remove(7);
            set.Remove(5);
            Assert.IsTrue(EqualSet(set, resultSet));
        }

        [Test]
        public void TestUnionWith()
        {
            int[] firstArr = { 1, 2, 3, 4, 5 };
            int[] secondArr = { 1, 2, 3,4 ,5, 7, 77 };
            Set<int> firstSet = new Set<int>(firstArr);
            firstSet.UnionWith(secondArr);
            

            Assert.IsTrue(EqualSet(firstSet, new Set<int>(new[] { 1, 2, 3, 4, 5, 7, 77 })));
        }

        public static bool EqualSet<T>(Set<T> rhs, Set<T> lhs,
            EqualityComparer<T> comparer = null)
        {
            if (rhs.Count != lhs.Count)
            {
                return false;
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            bool find = false;

            foreach (var rItem in rhs)
            {
                foreach (var lItem in lhs)
                {
                    if (comparer.Equals(lItem, rItem))
                    {
                        find = true;
                    }
                }

                if (!find)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
