using Microsoft.EntityFrameworkCore;
using Todoapi.Models;

namespace TodoApi.Models;

public class TodoContext : DbContext
{
    //TodoContextクラスのコンストラクタ
    //DbContextOptions<TodoContext>型のパラメータを受け取り、これを基底クラスのコンストラクタに渡す
    //DbContextOptionsは、DbContextの設定を指定するためのオプションを保持
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }
    //TodoItemsというDbSet<TodoItem>型のプロパティを定義
    //DbSetは、特定のエンティティタイプのコレクションを表し、データベースのテーブルに対応
    //このプロパティを通じて、データベースのTodoItemテーブルに対するCRUD操作を行える
    public DbSet<TodoItem> TodoItems { get; set; } = null!;
}
