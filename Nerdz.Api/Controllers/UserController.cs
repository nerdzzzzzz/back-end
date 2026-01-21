using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nerdz.Application.Commands.SyncUsers;
using Nerdz.Domain.Entities;
using System.Security.Claims;

namespace Nerdz.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ISender _mediator;

        public UserController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sync-me")]
        [ProducesResponseType(typeof(User), 201)] // Sucesso - Criado
        [ProducesResponseType(typeof(User), 200)] // Sucesso - Já existia
        [ProducesResponseType(401)] // Erro - Não autorizado
        public async Task<IActionResult> SyncUserOnFirstLogin()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("Token inválido.");
            }

            var email = User.FindFirstValue(ClaimTypes.Email);
            var nome = User.FindFirstValue("name"); // "name" é o claim padrão do Google/Apple

            var command = new SyncUserCommand
            {
                FirebaseUid = firebaseUid,
                Email = email,
                Nome = nome
            };
            var userProfile = await _mediator.Send(command);
            return Ok(userProfile);
        }
    }
}
