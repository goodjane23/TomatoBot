namespace TomatoBot.Services;

public class TomatoService
{
    public static int GetProcent()
    {
        Random r = new Random();
        return r.Next(0,101);
    }
}
