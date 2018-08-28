# LiteCache
基于.net core的轻量级缓存库,支持本地化缓存,任何对象都可以存储并且可设置有效时间,并且可以在停机后快速的恢复缓存.


### Nuget
https://www.nuget.org/packages/LiteCache/

```net cli 
dotnet add package LiteCache --version 1.0.5
```
```package manager  
Install-Package LiteCache -Version 1.0.5
```


### 示例

```c#
using System;
using Lsy.core.LiteCache;

namespace example {
    class Program {
        static void Main (string[] args) {
            var cache = new LiteCache<Person> (60, 10, "mycache");
            try {
                cache.InitLoadLocData ();
                foreach (var item in cache.GlobalDictionary) {
                    Console.WriteLine ($"location cache key={item.Key},name={item.Value.Name}");
                }
            } catch (System.Exception ex) {
                Console.WriteLine ("load location data error,inof=" + ex.Message);
            }

            while (true) {

                Console.WriteLine ("Please enter the keyboard number and press enter to confirm");
                Console.WriteLine ("1:creat,2:query,3:Count,4:Flush");
                var input_number = Console.ReadLine ();
                switch (input_number) {
                    case "1":
                        Console.WriteLine ("you creat data,For example:zj,30,0");
                        var input_creat = Console.ReadLine ();
                        var ary = input_creat.Split (',');
                        var newPerson = new Person () { Name = ary[0], Age = int.Parse (ary[1]), Gender = int.Parse (ary[2]) };
                        var set_result = cache.SetValue (newPerson);
                        if (set_result.Item2) {
                            Console.WriteLine ("creat data complete,guid=" + set_result.Item1);
                        } else {
                            Console.WriteLine ("creat data error");
                        }
                        break;
                    case "2":
                        Console.WriteLine ("you query data,input guid key");
                        var input_query = Console.ReadLine ();
                        var result = cache.GetValue (input_query);
                        if (result.Item2) {
                            Console.WriteLine ($"data query complete,name:{result.Item1.Name},age:{result.Item1.Age}");
                        } else {
                            Console.WriteLine ("not data for query");
                        }
                        break;
                    case "3":
                        Console.WriteLine ("data count=" + cache.Count);
                        break;
                    case "4":
                        if (cache.Flush ()) {
                            Console.WriteLine ("flush data complete");
                        }
                        break;
                }
            }
        }
    }

    [Serializable]
    public class Person {
        public string Name;
        public int Age;
        public int Gender;

    }
}
```

### 版权
本项目采用[MIT](http://opensource.org/licenses/MIT)开源授权许可证，完整的授权说明可在[LICENSE](LICENSE)文件中找到。
