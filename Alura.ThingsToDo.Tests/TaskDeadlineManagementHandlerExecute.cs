using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Alura.ThingsToDo.Tests
{
    public class TaskDeadlineManagementHandlerExecute
    {
        [Fact]
        public void WhenTasksAreOverdueTheyNeedToChangeTheirStatus()
        {
            //arranje
            var buyCategory = new Categoria(1, "Compras");
            var houseCategory = new Categoria(2, "Casa");
            var workCategory = new Categoria(3, "Work");

            var tasks = new List<Tarefa>
            {
                //tarefas atrasadas
                new Tarefa(2, "Tirar lixo", houseCategory, new DateTime(2022, 02, 07), null, StatusTarefa.Criada),
                new Tarefa(3, "Fazer o almoço", houseCategory, new DateTime(2022, 02, 06), null, StatusTarefa.Criada),

                //tarefas validas (dentro do prazo)
                new Tarefa(4, "Concluir relatório", workCategory, new DateTime(2022, 02, 20), null, StatusTarefa.Criada)
            };

            var options = new DbContextOptionsBuilder<DbTarefasContext>().UseInMemoryDatabase("DbTarefasContext").Options;
            var context = new DbTarefasContext(options);
            var repository = new RepositorioTarefa(context);

            repository.IncluirTarefas(tasks.ToArray());

            var command = new GerenciaPrazoDasTarefas(new DateTime(2022, 08, 02));
            var handler = new GerenciaPrazoDasTarefasHandler(repository);

            //act
            handler.Execute(command);

            //assert
            var tarefasEmAtraso = repository.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(3, tarefasEmAtraso.Count());
           
        }

        [Fact]
        public void WhenExecuteIsInvokedMustCallRefreshTasksInQuantityTimesOfTotalOfOverdueTasks()
        {
            //arrange
            var category = new Categoria("Test");

            var tasks = new List<Tarefa>
            {
                new Tarefa(1, "Tirar lixo", category, new DateTime(2022, 02, 07), null, StatusTarefa.Criada),
                new Tarefa(2, "Fazer o almoço", category, new DateTime(2022, 02, 06), null, StatusTarefa.Criada),
            };

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>())).Returns(tasks);

            var repository = mock.Object;

            var command = new GerenciaPrazoDasTarefas(new DateTime(2022, 02, 11));
            var handler = new GerenciaPrazoDasTarefasHandler(repository);

            //act
            handler.Execute(command);

            //assert
            mock.Verify(r => r.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once());
        }
    }
}
