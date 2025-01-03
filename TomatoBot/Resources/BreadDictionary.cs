namespace TomatoBot.Resources;

public static class BreadDictionary
{
    private static Dictionary<int, string> bread = new Dictionary<int, string>()
    {
        {0,"Пирожочек" },
        {1,"Булочка с повидлом" },
        {2,"Булочка с маком" },
        {3,"Булочка с корицей" },
        {4,"Булочка с шоколадной сгущенкой" },
        {5,"Плюшка" },
        {6,"Батон" },
        {7,"Багет" },
        {8,"Нарезаный батон" },
        {9,"Кирпич" },
        {10,"Ржаной" },
        {11,"Бородинский" },
        {12,"Крендель" },
        {13,"Круассан" },        
        {14,"Лаваш" },
        {15,"Пончик" },
        {16,"Чиабатта" }
    };
    public static Dictionary<int, string> Bread => bread;
}
