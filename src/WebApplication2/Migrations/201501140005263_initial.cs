using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Relational.Model;
using System;

namespace WebApplication2.Migrations
{
    public partial class initial : Migration
    {
        public override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable("TodoItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsDone = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        Title = c.String()
                    })
                .PrimaryKey("PK_TodoItem", t => t.Id);
        }
        
        public override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("TodoItem");
        }
    }
}