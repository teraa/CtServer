using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CtServer.Features.Presentations
{
    [ApiController]
    [Route("[controller]")]
    public class PresentationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PresentationsController(IMediator mediator)
            => _mediator = mediator;

        [HttpGet] public string Index() => "Hello";
    }
}
