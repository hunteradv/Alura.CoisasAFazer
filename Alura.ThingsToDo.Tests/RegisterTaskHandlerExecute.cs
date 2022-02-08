using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

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
    }
}
