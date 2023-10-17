using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class LogHelper
{
    internal static void Info(Type declaringType, string v)
    {
        Console.WriteLine(declaringType.FullName + " " + v);
    }
}
