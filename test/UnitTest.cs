using System;
using System.Collections.Generic;
using Lsy.core.LiteCache;
using Xunit;
namespace test {
    public class UnitTest {

        [Fact]
        public void Test_TimeSecondsDiff () {
            var dt_now = DateTime.Now;
            var dt_now_end = dt_now.AddSeconds (60);
            var diff = Utility.TimeSecondsDiff (dt_now, dt_now_end);
            Assert.True (diff == 60);
        }

        [Fact]
        public void Test_GetEndSubString () {
            var s1 = "fsgsdgsfdddeed|20180808 10:00:00";
            var result = Utility.GetEndSubString (s1, '|');
            Assert.True (result == "20180808 10:00:00");
        }

        [Fact]
        public void Test_SaveCache () {
            var dic = new Dictionary<string, string> ();

            for (int i = 0; i < 100; i++) {
                dic.Add ("aaaaaaaaaaabbbbb|" + i, "data" + i);
            }
            var re = Utility.SaveCache (dic, "myCache");
            Assert.True (re);

        }

        [Fact]
        public void Test_GetCache () {

            Dictionary<string, string> re_dic = Utility.DeserializeCache<string> ("myCache");
            for (int i = 0; i < 100; i++) {
                re_dic.TryGetValue ("aaaaaaaaaaabbbbb|" + i, out var b);
                Assert.True (b == "data" + i);
            }

        }

        [Fact]
        public void Test_SaveCache2 () {
            var dic = new Dictionary<string, Person> ();

            for (int i = 0; i < 100; i++) {
                dic.Add ("aaaaaaaaaaabbbbb|" + i, new Person () { Name = "a", Age = i });
            }
            var re = Utility.SaveCache (dic, "myCache2");
            Assert.True (re);
        }

        [Fact]
        public void Test_GetCache2 () {

            Dictionary<string, Person> re_dic = Utility.DeserializeCache<Person> ("myCache2");
            for (int i = 0; i < 100; i++) {
                re_dic.TryGetValue ("aaaaaaaaaaabbbbb|" + i, out var b);
                Assert.True (b.Age == i && b.Name == "a");
            }

        }
    }
    [Serializable]
    public class Person {
        public string Name;
        public int Age;

    }
}