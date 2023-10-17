using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Web;

public static class UmbracoHelperExtensions
{
    public static string If(this UmbracoHelper umbraco, bool test, string valueIfTrue, string valueIfFalse = "")
    {
        return test ? valueIfTrue : valueIfFalse;
    }

}

public static class Umbraco7
{
    public static string If(bool test, string valueIfTrue, string valueIfFalse = "")
    {
        return test ? valueIfTrue : valueIfFalse;
    }

}