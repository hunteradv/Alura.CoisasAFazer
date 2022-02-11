using System;
using System.Collections.Generic;
using System.Text;

namespace Alura.CoisasAFazer.Services.Handlers
{
    public class CommandResult
    {
        public bool IsSuccess { get; set; }

        public CommandResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
    }
}
