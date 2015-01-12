using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fish.CommLib;
using Fish.SSMSTools;

namespace Fish.MainRun
{
    class Program
    {
        static void Main(string[] args)
        {
            ScriptTool a = new ScriptTool("10.35.63.10", "sa", "p@ssw0rd", "VAV");

            a.GenAll();

            Console.WriteLine("done.........");
            Console.Read();
        }
    }

    /*
     
               byte[] byKey = new byte[] { 68, 183, 43, 71, 157, 254, 117, 240 };
              byte[] byIV = new byte[] { 194, 249, 70, 52, 59, 140, 165, 142 };
            string k = "12345678";
            var aa = "Hello,World.";

            var bb = CryptManager.Encode(aa );
            Console.WriteLine(bb);
            //zk9AhvAV5Jx/eSJs8TUngg==

            var cc = CryptManager.Encode(bb );
            Console.WriteLine(cc);
  
     
     */
}
