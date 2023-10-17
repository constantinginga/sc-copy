using System.Collections.Generic;

public class Wrapper
{
    public applinks applinks { get; set; }
}

public class applinks
{
    public List<string> apps { get; set; }
    public List<details> details { get; set; }
}

public class details
{
    public string appID { get; set; }
    public List<string> paths { get; set; }
}