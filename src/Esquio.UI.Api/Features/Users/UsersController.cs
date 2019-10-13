﻿using Esquio.UI.Api.Features.Users.Add;
using Esquio.UI.Api.Features.Users.List;
using Esquio.UI.Api.Features.Users.My;
using Esquio.UI.Api.Features.Users.Update;
using Esquio.UI.Api.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Esquio.UI.Api.Features.Users
{
    [Authorize]
    [ApiController]
    public class UsersController
        : Controller
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [Route("api/v1/users/my")]
        public async Task<IActionResult> My(CancellationToken cancellationToken = default)
        {
            var request = new MyRequest()
            {
                SubjectId = User.GetSubjectId()
            };

            var response = await _mediator.Send(request, cancellationToken);

            if (response.IsAuthorized)
            {
                return Ok(response);
            }

            return Forbid();
        }

        [HttpGet]
        [Authorize(Policies.Management)]
        [Route("api/v1/users")]
        public async Task<IActionResult> List([FromQuery]ListUsersRequest request,CancellationToken cancellationToken = default)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Policies.Management)]
        [Route("api/v1/users/permission")]
        public async Task<IActionResult> Add([FromBody]AddPermissionRequest request, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(request, cancellationToken);
            return Ok();
        }

        [HttpPut]
        [Authorize(Policies.Management)]
        [Route("api/v1/users/permission")]
        public async Task<IActionResult> Update([FromBody]UpdatePermissionRequest request, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(request, cancellationToken);
            return Ok();
        }
    }
}
