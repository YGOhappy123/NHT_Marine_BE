using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHT_Marine_BE.Data.Dtos.Response
{
    public class ErrorResponseDto
    {
        public string? Message { get; set; }
        public object? Error { get; set; }
    }
}
