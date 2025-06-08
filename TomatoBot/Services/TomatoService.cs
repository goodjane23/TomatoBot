
namespace TomatoBot.Services;

public class TomatoService
{
    public static int GetPercent()
    {
        Random random = new();
        int chance = random.Next(100);
        if (chance < 20) return 69;
        return random.Next(1, 101);
    }
}
