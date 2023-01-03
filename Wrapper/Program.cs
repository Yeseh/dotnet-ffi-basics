using Wrapper;

public static class Program
{
    public static void Main(string[] args)
    {
        var cObj = new CObject("test");
        var castInner1 = cObj.Inner as string;

        Console.WriteLine(cObj.Name);
        Console.WriteLine(castInner1);

        cObj.Inner = new int[] { 1, 2, 3 };
        var castInner2 = cObj.Inner as int[];
        

        //GC.Collect();
        //GC.WaitForPendingFinalizers();
        cObj.Dispose();

        Console.ReadLine();
    }
}