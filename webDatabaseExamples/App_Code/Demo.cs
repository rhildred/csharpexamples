using System;
/// <summary>
/// Demo is a do nothing class that demonstrates throwing an exception that can be handled at the correct level of abstraction
/// </summary>
public class Demo
{
    public int nDemo = 15;
    /// <summary>
    /// just throws an exception
    /// </summary>
    /// <returns>nothing ... only throws</returns>
    public int throws()
    {
        throw new Exception("I threw from the throw method");
    }
}