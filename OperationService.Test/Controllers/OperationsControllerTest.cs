using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OperationsService.Controllers;
using OperationsService.Data;
using OperationsService.Data.Models;
using Xunit;

namespace OperationService.Test.Controllers
{
    public class OperationsControllerTest
    {
        private readonly Fixture _fixture;
        private readonly Mock<ILogger<OperationsController>> _loggerMock;

        private readonly DbContextOptions<DatabaseContext> _dbContextOptions;


        public OperationsControllerTest()
        {
            _fixture = new Fixture();
            _loggerMock = new Mock<ILogger<OperationsController>>();
            _dbContextOptions =
                _fixture.Create<DbContextOptions<DatabaseContext>>();
        }

        [Fact]
        public void GetAll_Returns_All_Operations()
        {
            //Given
            var operations = new List<Operation>
            {
                _fixture.Create<Operation>(),
                _fixture.Create<Operation>()
            }.AsQueryable();

            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);

            //When
            var result = controller.Get();

            //Then
            Assert.Equal(operations.Count(), result.Count());
        }

        [Fact]
        public void Get_Returns_Operation_If_Exists()
        {
            //Given
            var operationId = _fixture.Create<int>();
            var operation = _fixture.Build<Operation>().With(operation => operation.Id, operationId).Create();
            var operations = new List<Operation>
            {
                operation
            }.AsQueryable();

            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);

            //When
            var result = controller.Get(operationId);

            //Then
            var okResult = result as OkObjectResult;
            Assert.Equal(operation, okResult.Value);
        }

        [Fact]
        public void Get_Returns_Status_404_If_Not_Exists()
        {
            //Given
            var operations = new List<Operation>().AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);
            var operationId = _fixture.Create<int>();
            //When
            var result = controller.Get(operationId);

            //Then
            var notFoundResult = result as NotFoundResult;
            Assert.True(notFoundResult.StatusCode == StatusCodes.Status404NotFound);
        }

        [Fact]
        public void Post_Returns_Operation_If_Added()
        {
            //Given
            var operations = new List<Operation>().AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);
            var operation = _fixture.Create<Operation>();

            //When
            var result = controller.Post(operation);
            var okResult = result as CreatedAtActionResult;

            //Then
            Assert.Equal(operation, okResult.Value);
        }

        [Fact]
        public void Post_Returns_Internal_Server_Error_If_Adding_Failed()
        {
            //Given
            var operations = new List<Operation>().AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);
            databaseContextMock.Setup(x => x.SaveChanges()).Throws(new DbUpdateException());

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);
            var operation = _fixture.Create<Operation>();

            //When
            var result = controller.Post(operation);

            //Then
            var codeResult = result as StatusCodeResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, codeResult.StatusCode);
        }

        [Fact]
        public void Put_Returns_Operation_If_Updated()
        {
            //Given
            var operationId = _fixture.Create<int>();
            var operation = _fixture.Build<Operation>().With(operation => operation.Id, operationId).Create();
            var operations = new List<Operation>
            {
                operation
            }.AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);
            var updatedOperation = _fixture.Build<Operation>().With(operation => operation.Id, operationId).Create();

            //When
            var result = controller.Put(operationId, updatedOperation);

            //Then
            var okResult = result as OkObjectResult;
            Assert.Equal(updatedOperation, okResult.Value);
        }

        [Fact]
        public void Put_Returns_Internal_Server_Error_If_Updating_Failed()
        {
            //Given
            var operationId = _fixture.Create<int>();
            var operation = _fixture.Build<Operation>().With(operation => operation.Id, operationId).Create();
            var operations = new List<Operation>
            {
                operation
            }.AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);
            databaseContextMock.Setup(x => x.SaveChanges()).Throws(new DbUpdateException());

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);
            var updatedOperation = _fixture.Build<Operation>().With(operation => operation.Id, operationId).Create();

            //When
            var result = controller.Put(operationId, updatedOperation);

            //Then
            var codeResult = result as StatusCodeResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, codeResult.StatusCode);
        }

        [Fact]
        public void Put_Returns_Not_Found_Error_If_Operation_Not_Found()
        {
            //Given
            var operations = new List<Operation>
            {
            }.AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);
            var operationId = _fixture.Create<int>();
            var updatedOperation = _fixture.Build<Operation>().With(operation => operation.Id, operationId).Create();

            //When
            var result = controller.Put(operationId, updatedOperation);

            //Then
            var notFoundResult = result as NotFoundResult;
            Assert.True(notFoundResult.StatusCode == StatusCodes.Status404NotFound);
        }

        [Fact]
        public void Delete_Returns_Internal_Server_Error_If_Deleting_Failed()
        {
            //Given
            var operations = new List<Operation>
            {
            }.AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);
            databaseContextMock.Setup(x => x.SaveChanges()).Throws(new DbUpdateException());

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);

            //When
            var result = controller.Delete(_fixture.Create<int>());

            //Then
            var codeResult = result as StatusCodeResult;
            Assert.Equal(StatusCodes.Status500InternalServerError, codeResult.StatusCode);
        }

        [Fact]
        public void Delete_Returns_Status_Ok_If_Deleted()
        {
            //Given
            var operationId = _fixture.Create<int>();
            var operation = _fixture.Build<Operation>().With(operation => operation.Id, operationId).Create();
            var operations = new List<Operation>
            {
                operation
            }.AsQueryable();
            var operationsMock = CreateDbSetMock(operations);
            var databaseContextMock = new Mock<DatabaseContext>(_dbContextOptions);
            databaseContextMock.Setup(x => x.Operations).Returns(operationsMock.Object);

            var controller = new OperationsController(_loggerMock.Object, databaseContextMock.Object);

            //When
            var result = controller.Delete(operationId);

            //Then
            var okResult = result as OkResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        }

        private static Mock<DbSet<T>> CreateDbSetMock<T>(IEnumerable<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());

            return dbSetMock;
        }
    }
}
