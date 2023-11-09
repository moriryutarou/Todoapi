using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
//アプリケーションのホストとデフォルトのアプリケーションを作成
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//HTTPリクエストの処理、モデルバインディング等が使用できる
builder.Services.AddControllers();

//TodoContextの新しいインスタンスを作成、既存性注入コンテナに追加する
builder.Services.AddDbContext<TodoContext>(opt =>
//DBプロバイダとしてインメモリDBを使用するようにEntity Framework Coreを設定する
    opt.UseInMemoryDatabase("TodoList"));

//エンドポイントに関する情報を公開するサービスの登録
builder.Services.AddEndpointsApiExplorer();

//Swaggerドキュメントの生成機能をアプリケーションに追加する
builder.Services.AddSwaggerGen();

//アプリケーションのホストを構築し、アプリケーションのインスタンスを作成する
var app = builder.Build();

// Configure the HTTP request pipeline.
//現在の環境が開発環境かどうかを判断する
if (app.Environment.IsDevelopment())
{
    //Swagger JSONエンドポイントの有効化
    app.UseSwagger();
    //Swagger UIの有効化
    app.UseSwaggerUI();
}

//HTTPリクエストをHTTPSにリダイレクトするミドルウェアを追加する
app.UseHttpsRedirection();

//認証ミドルウェアをパイプラインに追加
///app.UseAuthorization();

//コントローラーベースのエンドポイントをエンドポイントルーティングにマッピング
//特定のURLパスが指定されたコントローラーとアクションメソッドにルーティングされる
app.MapControllers();

//アプリケーションの実行
app.Run();
