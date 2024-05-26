namespace TomatoBot.Services;

public class TomatoService
{
    public static int GetPercent()
    {
        Random r = new();
        return r.Next(0,101);
    }
}
