using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
//�A�v���P�[�V�����̃z�X�g�ƃf�t�H���g�̃A�v���P�[�V�������쐬
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//HTTP���N�G�X�g�̏����A���f���o�C���f�B���O�����g�p�ł���
builder.Services.AddControllers();

//TodoContext�̐V�����C���X�^���X���쐬�A�����������R���e�i�ɒǉ�����
builder.Services.AddDbContext<TodoContext>(opt =>
//DB�v���o�C�_�Ƃ��ăC��������DB���g�p����悤��Entity Framework Core��ݒ肷��
    opt.UseInMemoryDatabase("TodoList"));

//�G���h�|�C���g�Ɋւ���������J����T�[�r�X�̓o�^
builder.Services.AddEndpointsApiExplorer();

//Swagger�h�L�������g�̐����@�\���A�v���P�[�V�����ɒǉ�����
builder.Services.AddSwaggerGen();

//�A�v���P�[�V�����̃z�X�g���\�z���A�A�v���P�[�V�����̃C���X�^���X���쐬����
var app = builder.Build();

// Configure the HTTP request pipeline.
//���݂̊����J�������ǂ����𔻒f����
if (app.Environment.IsDevelopment())
{
    //Swagger JSON�G���h�|�C���g�̗L����
    app.UseSwagger();
    //Swagger UI�̗L����
    app.UseSwaggerUI();
}

//HTTP���N�G�X�g��HTTPS�Ƀ��_�C���N�g����~�h���E�F�A��ǉ�����
app.UseHttpsRedirection();

//�F�؃~�h���E�F�A���p�C�v���C���ɒǉ�
///app.UseAuthorization();

//�R���g���[���[�x�[�X�̃G���h�|�C���g���G���h�|�C���g���[�e�B���O�Ƀ}�b�s���O
//�����URL�p�X���w�肳�ꂽ�R���g���[���[�ƃA�N�V�������\�b�h�Ƀ��[�e�B���O�����
app.MapControllers();

//�A�v���P�[�V�����̎��s
app.Run();
