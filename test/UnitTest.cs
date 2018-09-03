using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Lsy.core.LiteCache;
using ProtoBuf;
using Xunit;
using Xunit.Abstractions;

namespace test {
    public class UnitTest {

        [Fact (Skip = "跳过测试")]
        public void Test_TimeSecondsDiff () {
            var dt_now = DateTime.Now;
            var dt_now_end = dt_now.AddSeconds (60);
            var diff = Utility.TimeSecondsDiff (dt_now, dt_now_end);
            Assert.True (diff == 60);
        }

        [Fact (Skip = "跳过测试")]
        public void Test_GetEndSubString () {
            var s1 = "fsgsdgsfdddeed|20180808 10:00:00";
            var result = Utility.GetEndSubString (s1, '|');
            Assert.True (result == "20180808 10:00:00");
        }

        [Theory (DisplayName = "单条保存测试")]
        [InlineData ("zj1", 1, 0)]
        [InlineData ("zj2", 1, 0)]
        [InlineData ("zj3", 1, 0)]
        [InlineData ("zj4", 1, 0)]
        [InlineData ("zj5", 1, 0)]
        [InlineData ("zj6", 1, 0)]
        public void Test_Save (string name, int age, int gender) {
            var p1 = new Person () { Name = name, Age = age, Gender = gender };
            var cache = new LiteCache<Person> (99999, 99999, "mycache");
            var item = cache.AddValue (p1);
            Assert.True (!string.IsNullOrEmpty (item.Item1) && item.Item2);
        }

        [Fact (DisplayName = "批量保存测试")]
        public void Test_BatchSave () {
            var cache = new LiteCache<Person> (99999, 99999, "mycache1");
            var dataList = new List<Person> ();
            for (int i = 0; i < 100; i++) {
                var p = new Person () { Name = "testName", Age = i, Gender = i };
                dataList.Insert (0, p);
            }
            var item = cache.AddValue (dataList);
            Assert.True (item.Item1 == 100 && item.Item2);
        }

        [Fact (DisplayName = "数据变化测试")]
        public void Test_changeData () {
            var cache = new LiteCache<Person> (99999, 99999, "mycache3");
            Assert.True (cache.Count == 0);
            var data1 = cache.AddValue (new Person () { Name = "zj1", Age = 30, Gender = 0 });
            var data2 = cache.AddValue (new Person () { Name = "zj2", Age = 30, Gender = 0 });
            var data3 = cache.AddValue (new Person () { Name = "zj3", Age = 30, Gender = 0 });
            Assert.True ("zj1" == cache.GlobalDictionary[data1.Item1].Name);
            Assert.True ("zj2" == cache.GlobalDictionary[data2.Item1].Name);
            Assert.True ("zj3" == cache.GlobalDictionary[data3.Item1].Name);

            var cache_copy = new LiteCache<Person> (99999, 99999, "mycache3");
            Assert.True (cache_copy.Count == 3);

            Assert.True (cache_copy.DeleteValue (data1.Item1));

            Assert.True (cache_copy.Count == 2);

            Assert.True (cache_copy.UpdateValue (data2.Item1, new Person () { Name = "zj new", Age = 30, Gender = 0 }));

            Assert.True (cache_copy.GetValue (data2.Item1).Item1.Name == "zj new");

            Assert.True (cache_copy.Flush ());
        }

        [Theory (DisplayName = "数据清空测试")]
        [InlineData (5, 1, 4)]
        [InlineData (5, 1, 6)]
        public async Task Test_dataClear (int liveTime, int checkTime, int delayTime) {
            var cache = new LiteCache<Person> (liveTime, checkTime, "mycache4");
            var task1 = await cache.AsynAddValue (new Person () { Name = "zj", Age = 30, Gender = 0 });
            Assert.True (task1.Item2 && !string.IsNullOrEmpty (task1.Item1));
            await Task.Delay (delayTime * 1000);
            var task2 = await cache.AsynGetValue (task1.Item1);
            if (delayTime > liveTime) {
                Assert.True (!task2.Item2 && task2.Item1 == null);
            } else {
                Assert.True (task2.Item2 && task2.Item1.Name == "zj");
            }
            cache.Flush ();
        }
    }

    [ProtoContract]
    public class Person {
        public string Name;

        public int Age;

        public int Gender;

    }
}