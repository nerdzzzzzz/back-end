using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;

namespace Nerdz.Application.Commands.DeleteGroup
{
    public class DeleteGroupCommand : IRequest
    {
        public string FirebaseUid { get; set; } = string.Empty;
    }

    public class DeleteGroupCommandValidator : AbstractValidator<DeleteGroupCommand>
    {
        public DeleteGroupCommandValidator()
        {
            RuleFor(cmd => cmd.FirebaseUid)
                .NotEmpty()
                .WithMessage("O ID do usuário (UID) é obrigatório.");
        }
    }
}
