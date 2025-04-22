using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Data;
using StudentManagement.Models;
using StudentManagement.Controllers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace StudentManagementTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task GetStudents_ReturnsAllStudents()
        {
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentDbTest")
                .Options;

            using (var context = new StudentContext(options))
            {
                context.Students.Add(new Student { Id = 1, FirstName = "test", LastName = "testset", Age = 40 });
                context.Students.Add(new Student { Id = 2, FirstName = "testo", LastName = "testotset", Age = 50 });
                await context.SaveChangesAsync();
            }

            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);

                var result = await controller.GetStudents();

                var actionResult = Assert.IsType<ActionResult<IEnumerable<Student>>>(result);
                var studentsList = Assert.IsAssignableFrom<IEnumerable<Student>>(actionResult.Value);

                Assert.Equal(2, studentsList.Count());
            }
        }

        [Fact]
        public async Task GetStudent_ReturnsNotFound_WhenStudentDoesNotExist()
        {
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentDbTest_NotFound")
                .Options;

            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);

                var result = await controller.GetStudent(99);

                Assert.IsType<NotFoundResult>(result.Result);
            }
        }

        [Fact]
        public async Task PostStudent_ReturnsAddedStudent()
        {
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentDbTest_StudentCreated")
                .Options;

            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);
                var newStudent = new Student { Id = 4, FirstName = "Testi", LastName = "Testi", Age = 30 };

                await controller.PostStudent(newStudent);

                var result = await controller.GetStudent(4);

                // Assert
                Assert.Equal(newStudent, result.Value);

            }
        }

        [Fact]
        public async Task DeleteStudent_ReturnsNotFound_IfNonExistent()
        {
            var options = new DbContextOptionsBuilder<StudentContext>()
                .UseInMemoryDatabase(databaseName: "StudentDbTest_DeletedStudent")
                .Options;

            using (var context = new StudentContext(options))
            {
                var controller = new StudentsController(context);

                var newStudent = new Student { Id = 5, FirstName = "Pena", LastName = "Poisto", Age = 60 };

                await controller.PostStudent(newStudent);
                await controller.DeleteStudent(5);

                var result = await controller.GetStudent(5);

                Assert.IsType<NotFoundResult>(result.Result);
            }
        }
    }
}