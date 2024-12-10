﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course.API.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrerequisiteCourseName",
                table: "CoursePrerequisites",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrerequisiteCourseName",
                table: "CoursePrerequisites");
        }
    }
}