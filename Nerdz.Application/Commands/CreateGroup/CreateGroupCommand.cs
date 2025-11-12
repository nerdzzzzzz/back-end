using FluentValidation;
using MediatR;
using Nerdz.Domain.Entities;
using System.Text.Json.Serialization;

namespace Nerdz.Application.Commands.CreateGroup
{
    public class CreateGroupCommand : IRequest<GroupProfile>
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [JsonIgnore]
        public string FirebaseUid { get; set; } = string.Empty;
    }

    public class CreateGroupCommandValidator : AbstractValidator<CreateGroupCommand>
    {
        public CreateGroupCommandValidator()
        {
            RuleFor(cmd => cmd.Nome)
                .NotEmpty()
                .WithMessage("O nome do grupo é obrigatório.");

            RuleFor(cmd => cmd.FirebaseUid)
                .NotEmpty()
                .WithMessage("O ID do usuário (UID) é obrigatório.");
        }
    }
}
