namespace Todoapi.Models
{
    //TodoItemDTOクラスを定義
    //DTOはData Transfer Objectの略で、異なるレイヤー間でデータを転送するためのオブジェクト
    public class TodoItemDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
