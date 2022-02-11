using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;

namespace Alura.ThingsToDo.Tests
{
    public class RegisterTaskHandlerExecute
    {
        [Fact]
        public void RegisterTaskInDbWhenInsertTaskWithValidInformations()
        {
            //arranje
            var command = new CadastraTarefa("Estudar .net", new Categoria("Estudos"), new DateTime(2022, 02, 03));

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;
            var context = new DbTarefasContext(options);
            var repository = new RepositorioTarefa(context);

            var handler = new CadastraTarefaHandler(repository);

            //act
            handler.Execute(command);

            //assert
            var tasks = repository.ObtemTarefas(t => t.Titulo == "Estudar .net").FirstOrDefault();
            Assert.NotNull(tasks);
        }

        [Fact]
        public void WhenExceptionIsThrowResultIsSuccessMustBeFalse()
        {
            //arranje
            var command = new CadastraTarefa("Estudar .net", new Categoria("Estudos"), new DateTime(2022, 02, 03));

            var mock = new Mock<IRepositorioTarefas>();

            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repository = mock.Object;

            var handler = new CadastraTarefaHandler(repository);

            //act
            CommandResult result = handler.Execute(command);

            //assert
            Assert.False(result.IsSuccess);            
        }
    }
}
