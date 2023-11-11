using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Todoapi.Models;
using TodoApi.Models;

namespace TodoApi.Controllers;
//TodoItemsControllerクラスを定義
//[controller]がTodoItemsに置き換えられる
//このコントローラーのエンドポイントはapi/TodoItemsから始まる
[Route("api/[controller]")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    //依存性注入(DI)を使用してデータベースコンテキスト(今回はTodoContext)にコントローラーを注入
    //DBとやり取りを行うためのメソッドを使用することができる
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
        _context = context;
    }

    // GET: api/TodoItems
    [HttpGet]
    //Task<ActionResult<IEnumerable<TodoItemDTO>>>は戻り値の型
    //非同期操作の結果としてActionResult<IEnumerable<TodoItemDTO>>型の値を返す
    //asyncはこのメソッドが非同期であることを示している
    //ActionResultはHTTP応答を表し、IEnumerable<TodoItemDTO>はTodo項目のDTOコレクションを表す
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetTodoItems()
    {
        //_ContextからTodo項目のコレクション(TodoItems)を非同期に取得する
        //awaitは非同期処理の完了を待つことを示している
        return await _context.TodoItems
            //LINQを使用し、取得した各Todo項目(ｘ)をDTOに変換する
            //これによりデータベースエンティティからクライアントに送信するためのデータモデルに変換される
            .Select(x => ItemToDTO(x))
            //変換されたTodo項目のDTOのコレクションを非同期にリストとして取得、リストはHTTP応答として返される
            .ToListAsync();
    }

    // GET: api/TodoItems/5
    // <snippet_GetByID>
    [HttpGet("{id}")]
    //引数としてTodo項目のID（long id)を受け取り、
    //非同期処理の結果として、<ActionResult<TodoItemDTO>>型の値を返す
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
    {
        //_contextから指定されたID(id)に対応するTodo項目を非同期に探して
        //見つかった項目を変数todoItemに格納する
        var todoItem = await _context.TodoItems.FindAsync(id);

        //指定されたIDのTodo項目がDBに存在しない場合に
        //404NotFoundステータスコードを含むHTTP応答を返す
        if (todoItem == null)
        {
            return NotFound();
        }
        //見つかったTodo項目(todoItem)をDTOに変換し、結果をHTTP応答として返す。
        return ItemToDTO(todoItem);
    }
    // </snippet_GetByID>

    // PUT: api/TodoItems/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    // <snippet_Update>
    [HttpPut("{id}")]
    //引数としてTodo項目のID(id)と更新されたTodo項目のデータ（TodoItemDTO todoDTO）を受け取り
    //非同期操作の結果としてActionResult型の値を返す
    public async Task<IActionResult> PutTodoItem(long id, TodoItemDTO todoDTO)
    {
        //URLパスから受け取ったIDと更新されたデータから受け取ったIDが一致しない場合、
        //400 Bad Requestステータスを含むHTTP応答を返す
        if (id != todoDTO.Id)
        {
            return BadRequest();
        }
        //_contextから指定されたID(id)に対応するTodo項目を非同期に探して
        //見つかった項目を変数todoItemに格納する
        var todoItem = await _context.TodoItems.FindAsync(id);
        //指定されたIDのTodo項目がDBに存在しない場合に
        //404NotFoundステータスコードを含むHTTP応答を返す
        if (todoItem == null)
        {
            return NotFound();
        }
        //見つかったTodo項目(todoItem)のプロパティ(NameとIsComplete)を、
        //更新されたデータ(todoDTO)から取得した値で更新する
        todoItem.Name = todoDTO.Name;
        todoItem.IsComplete = todoDTO.IsComplete;

        //_Contextに対して非同期に保存操作(SaveChangesAsync)を呼び出し、
        //更新されたTodo項目をDBに保存する
        try
        {
            await _context.SaveChangesAsync();
        }
        //他のユーザーが同時に同じTodo項目を更新しようとした場合等に発生する可能性のある
        //DbUpdateConcurrencyException例外を受け取り、
        //かつ指定されたIDのTodo項目がDBに存在しなければ404 NotFoundを返す
        catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        {
            return NotFound();
        }

        //更新操作が成功した場合、204 No Contentを含むHTTP応答を返す
        return NoContent();
    }
    // </snippet_Update>

    // POST: api/TodoItems
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    // <snippet_Create>
    [HttpPost]
    //引数として新たに作成するTodo項目のデータ(TodoItemDTO todoDTO)を受け取り
    //非同期操作の結果としてActionResult<TodoItemDTO>型の値を返す
    public async Task<ActionResult<TodoItemDTO>> PostTodoItem(TodoItemDTO todoDTO)
    {
        //新たに作成するTodo項目を生成する
        //IsComplete、Name、Secretは引数から受け取ったtodoDTOから取得した値で設定される
        var todoItem = new TodoItem
        {
            IsComplete = todoDTO.IsComplete,
            Name = todoDTO.Name,
        };

        //_contextに対して新たに作成したTodo項目(todoItem)に追加する
        _context.TodoItems.Add(todoItem);
        //_Contextに対して非同期に保存操作(SaveChangesAsync)を呼び出し、
        //更新されたTodo項目をDBに保存する
        await _context.SaveChangesAsync();

        //新たに作成されたTodo項目が正常にDBに保存された場合、
        //201CreatedとTodo項目のデータを含むHTTP応答を返す
        //CreatedAtActionメソッドには三つの引数を受け取る
        //nameof(GetTodoItem)：新たに作成したTodo項目を取得するためのアクションの名前
        //new { id = todoItem.Id }：アクションへのルート値。ここでは新たに作成したTodo項目のID。
        //ItemToDTO(todoItem)：HTTP応答本体。ここでは新たに作成したTodo項目のデータ。
        return CreatedAtAction(
            nameof(GetTodoItem),
            new { id = todoItem.Id },
            ItemToDTO(todoItem));
    }

    // DELETE: api/TodoItems/5
    [HttpDelete("{id}")]
    //引数としてID(long id)を受け取り、非同期操作の結果としてActionResult型の値を返す
    public async Task<IActionResult> DeleteTodoItem(long id)
    {
        //_contextから指定されたID(id)に対応するTodo項目を非同期に探して
        //見つかった項目を変数todoItemに格納する
        var todoItem = await _context.TodoItems.FindAsync(id);

        //指定されたIDのTodo項目がDBに存在しない場合に
        //404NotFoundステータスコードを含むHTTP応答を返す
        if (todoItem == null)
        {
            return NotFound();
        }

        //_contextから見つかったTodo項目(todoItem)を削除する
        _context.TodoItems.Remove(todoItem);
        //_contextに対して非同期に保存操作(SaveChangesAsync)を呼び出し削除したデータの変更をDBに保存する
        await _context.SaveChangesAsync();

        //削除に成功した場合、204 No Cotentを含むHTTP応答を返す
        return NoContent();
    }

    //引数としてTodo項目のID(long id)を受け取り、bool型の値を返す
    //このメソッドは指定されたIDのデータがDBに存在するかを確認する為に使用する
    private bool TodoItemExists(long id)
    {
        //_contextからTodoItemsを取得し、その中から指定したID(id)に対応する項目が存在するかを確認する
        return _context.TodoItems.Any(e => e.Id == id);
    }

    //引数としてTodo項目（TodoItem todoItem）を受け取り、
    //その結果としてTodo項目のDTO（Data Transfer Object)(TodoItemDTO)を返す
    private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
       //新たなTodo項目のDTO(new TodoItemDTO)を生成
       //(Id, Name, IsComplete)は引数から受け取ったtodoItemから取得した値で設定される
       new TodoItemDTO
       {
           Id = todoItem.Id,
           Name = todoItem.Name,
           IsComplete = todoItem.IsComplete,
       };
}