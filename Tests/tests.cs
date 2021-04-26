using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Processing
{
    public class Program
    {
        public static void Main()
        {
            string trans = "[\"1 тарелка \",\"2 тарелка \",\"3 тарелка \"]";
            Console.WriteLine(trans.Length);
            object json = JsonConvert.DeserializeObject<string[]>( trans );
            Console.WriteLine(json);
        }
    }
}
