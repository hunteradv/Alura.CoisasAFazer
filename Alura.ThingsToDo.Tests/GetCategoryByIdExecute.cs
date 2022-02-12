using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Infrastructure;

namespace Alura.ThingsToDo.Tests
{
    public class GetCategoryByIdExecute
    {
        [Fact]
        public void WhenIdExistsMustExecuteGetCategoryByIdOnce()
        {
            //arrange
            var categoryId = 20;

            var command = new ObtemCategoriaPorId(categoryId);
            
            var mock = new Mock<IRepositorioTarefas>();
            var repository = mock.Object;

            var handler = new ObtemCategoriaPorIdHandler(repository);

            //act
            handler.Execute(command);

            //assert
            mock.Verify(r => r.ObtemCategoriaPorId(categoryId), Times.Once);
        }
    }
}
