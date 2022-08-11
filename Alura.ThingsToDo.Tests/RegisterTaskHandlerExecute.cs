using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace Alura.ThingsToDo.Tests
{
    public class RegisterTaskHandlerExecute
    {
        [Fact]
        public void RegisterTaskInDbWhenInsertTaskWithValidInformations()
        {
            //arranje
            var command = new CadastraTarefa("Estudar .net", new Categoria("Estudos"), new DateTime(2023, 02, 03));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;
            var context = new DbTarefasContext(options);
            var repository = new RepositorioTarefa(context);

            var handler = new CadastraTarefaHandler(repository, mockLogger.Object);

            //act
            handler.Execute(command);

            //assert
            var tasks = repository.ObtemTarefas(t => t.Titulo == "Estudar .net").FirstOrDefault();
            Assert.NotNull(tasks);
        }

        delegate void CaptureMessageLog(LogLevel logLevel, EventId eventId, object state, Exception exception, Func<object, Exception, string> function);

        [Fact]
        public void WhenInsertTaskWithValidInformationsMustRegisterInLog()
        {
            //arrange
            var expectedTaskTitle = "Usar Moq para aprofundar conhecimento de API";

            var command = new CadastraTarefa(expectedTaskTitle, new Categoria("Estudos"), new DateTime(2023, 02, 03));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            LogLevel capturedLevel = LogLevel.Error;
            string capturedMessage = String.Empty;

            CaptureMessageLog capture = (logLevel, eventId, state, exception, function) =>
            {
                capturedLevel = logLevel;
                capturedMessage = function(state, exception);
            };

            mockLogger.Setup(l =>
                l.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    (Func<object, Exception, string>)It.IsAny<object>())
                ).Callback(capture);

            var mock = new Mock<IRepositorioTarefas>();
            
            var handler = new CadastraTarefaHandler(mock.Object, mockLogger.Object);

            //act
            handler.Execute(command);

            //assert
            Assert.Equal(LogLevel.Debug, capturedLevel);
            Assert.Contains(expectedTaskTitle, capturedMessage);
        }

        [Fact]
        public void WhenExceptionIsThrowResultIsSuccessMustBeFalse()
        {
            //arranje
            var command = new CadastraTarefa("Estudar .net", new Categoria("Estudos"), new DateTime(2023, 02, 03));

            var mock = new Mock<IRepositorioTarefas>();

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repository = mock.Object;

            var handler = new CadastraTarefaHandler(repository, mockLogger.Object);

            //act
            CommandResult result = handler.Execute(command);

            //assert
            Assert.False(result.IsSuccess);            
        }

        [Fact]
        public void WhenExceptionIsThrowMustLogTheMessageOfTheException()
        {
            var expectedMessage = "Houve um erro na inclusão de tarefas";
            var expectedException = new Exception(expectedMessage);

            //arranje
            var command = new CadastraTarefa("Estudar .net", new Categoria("Estudos"), new DateTime(2023, 02, 03));

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(expectedException);

            var repository = mock.Object;

            var handler = new CadastraTarefaHandler(repository, mockLogger.Object);

            //act
            CommandResult result = handler.Execute(command);

            //assert
            mockLogger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    expectedException,
                    (Func<object, Exception, string>)It.IsAny<object>()),
                Times.Once());
        }
    }
}
