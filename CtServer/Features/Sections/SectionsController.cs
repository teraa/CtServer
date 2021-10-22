using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Sections
{
    [ApiController]
    [Route("[controller]")]
    public class SectionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SectionsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet] public string Index() => "Hello";
    }
}
