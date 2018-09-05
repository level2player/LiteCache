using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Lsy.core.LiteCache;
using ProtoBuf;

namespace example {
    class Program {
        static void Main (string[] args) {
            var cache = new LiteCache<Person> (999999, 999999, "mycache");
            // try {
            //     foreach (var item in cache.GlobalDictionary) {
            //         Console.WriteLine ($"location cache key={item.Key},name={item.Value.Name}");
            //     }
            // } catch (System.Exception ex) {
            //     Console.WriteLine ("load location data error,info=" + ex.Message);
            // }

            // while (true) {

            //     Console.WriteLine ("Please enter the keyboard number and press enter to confirm");
            //     Console.WriteLine ("1:creat,2:query,3:Count,4:Flush");
            //     var input_number = Console.ReadLine ();
            //     switch (input_number) {
            //         case "1":
            //             Console.WriteLine ("you creat data,For example:zj,30,0");
            //             var input_creat = Console.ReadLine ();
            //             var ary = input_creat.Split (',');
            //             var newPerson = new Person () { Name = ary[0], Age = int.Parse (ary[1]), Gender = int.Parse (ary[2]) };
            //             var set_result = cache.AddValue (newPerson);
            //             if (set_result.Item2) {
            //                 Console.WriteLine ("creat data complete,guid=" + set_result.Item1);
            //             } else {
            //                 Console.WriteLine ("creat data error");
            //             }
            //             break;
            //         case "2":
            //             Console.WriteLine ("you query data,input guid key");
            //             var input_query = Console.ReadLine ();
            //             var result = cache.GetValue (input_query);
            //             if (result.Item2) {
            //                 Console.WriteLine ($"data query complete,name:{result.Item1.Name},age:{result.Item1.Age}");
            //             } else {
            //                 Console.WriteLine ("not data for query");
            //             }
            //             break;
            //         case "3":
            //             Console.WriteLine ("data count=" + cache.Count);
            //             break;
            //         case "4":
            //             if (cache.Flush ()) {
            //                 Console.WriteLine ("flush data complete");
            //             }
            //             break;
            //     }
            // }    

            Task t1 = Task.Run (() => {
                var list=new List<Person>();
                for (int i = 0; i < 999999; i++) {
                    list.Add (new Person () { Name = "zj1", Age = i, Gender = 0 });
                }
                cache.AddValue(list);
            });
        
        Task t2 = Task.Run (() => {
            // for (int i = 0; i < 99999; i++) {
            //     cache.AddValue (new Person () { Name = "zj2", Age = i, Gender = 0 });
            // }

        });

        var sw = Stopwatch.StartNew ();
        Task[] tasks = new Task[] { t1, t2 };
        Stopwatch.StartNew ();
        Task.WaitAll (tasks);
        Console.WriteLine ("count=" + cache.Count);
        Console.WriteLine (sw.Elapsed);
    }

    private static long GetObjectSize (object o) {
        using (var stream = new MemoryStream ()) {
            var formatter = new BinaryFormatter ();
            formatter.Serialize (stream, o);
            using (var fileStream = new FileStream ("size", FileMode.OpenOrCreate, FileAccess.Write)) {
                var buffer = stream.ToArray ();
                fileStream.Write (buffer, 0, buffer.Length);
                fileStream.Flush ();
            }

            return stream.Length;
        }
    }
}

[ProtoContract]
public class Person {
    public string Name;
    public int Age;
    public int Gender;

}

[Serializable]
public class Data {
    public string col_1;
    public string col_2;
    public string col_3;
    public string col_4;
    public string col_5;
    public string col_6;
    public string col_7;
    public string col_8;
    public string col_9;
    public string col_10;
    public string col_11;
    public string col_12;
    public string col_13;
    public string col_14;

    public string col_15;
}

public class ProductRowComparer : IEqualityComparer<Data> {
    public bool Equals (Data t1, Data t2) {
        return (t1.col_1 == t2.col_1 && t2.col_1 == "001");
    }
    public int GetHashCode (Data t) {
        return t.ToString ().GetHashCode ();
    }
}

}