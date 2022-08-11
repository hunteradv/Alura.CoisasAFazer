using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Alura.ThingsToDo.Tests
{
    public class TarefasControllerEndpointCadastraTarefa
    {
        [Fact]
        public void WhenTaskIsInsertedWithValidInfoMustReturn200()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasContext")
                .Options;
            var context = new DbTarefasContext(options);

            context.Categorias.Add(new Categoria(20, "Estudos"));
            context.SaveChanges();

            var repository = new RepositorioTarefa(context);

            var controller = new TarefasController(mockLogger.Object, repository);
            var model = new CadastraTarefaVM()
            {
                IdCategoria = 20,
                Titulo = "Estudar xUnit",
                Prazo = new DateTime(2022, 12, 20)
            };

            //act
            var postReturn = controller.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<OkResult>(postReturn); //200
        }

        [Fact]
        public void WhenExceptionIsThrownMustReturnCode500()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudos"));
            mock.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));

            var repository = mock.Object;

            var controller = new TarefasController(mockLogger.Object, repository);
            var model = new CadastraTarefaVM()
            {
                IdCategoria = 20,
                Titulo = "Estudar xUnit",
                Prazo = new DateTime(2022, 12, 20)
            };

            //act
            var postReturn = controller.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<StatusCodeResult>(postReturn);
            var statusCode = (postReturn as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCode);
        }
    }
}
