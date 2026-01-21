using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nerdz.Application.Commands.CreateGroup;
using Nerdz.Application.Commands.DeleteGroup;
using Nerdz.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Security.Claims;

namespace Nerdz.Api.Controllers
{
    [ApiController]
    [Route("api/groups")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly ISender _mediator;

        public GroupController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-group")]
        [ProducesResponseType(typeof(Group), 200)]
        [ProducesResponseType(400)] // Erro de validação (ex: já está em grupo)
        [ProducesResponseType(401)] // Não autorizado
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(firebaseUid))
                return Unauthorized("Token inválido.");

            command.FirebaseUid = firebaseUid;

            try
            {
                var newGroup = await _mediator.Send(command);
                return Ok(newGroup);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("delete-group")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)] // Erro de validação (ex: já está em grupo)
        [ProducesResponseType(401)] // Não autorizado
        public async Task<IActionResult> DeleteGroup()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(firebaseUid))
                return Unauthorized("Token inválido.");
            var request = new DeleteGroupCommand()
            {
                FirebaseUid = firebaseUid
            };

            try
            {
                await _mediator.Send(request);
                return Ok();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
