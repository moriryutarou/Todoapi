namespace Todoapi.Models
{
    //データベースのテーブルに対応するエンティティを表す
    public class TodoItem
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}

