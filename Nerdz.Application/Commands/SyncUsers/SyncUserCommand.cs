using FluentValidation;
using MediatR;
using Nerdz.Domain.Entities;

namespace Nerdz.Application.Commands.SyncUsers
{
    public class SyncUserCommand : IRequest<UserProfile>
    {
        public string FirebaseUid { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
    }

    public class SyncUserCommandValidator : AbstractValidator<SyncUserCommand>
    {
        public SyncUserCommandValidator()
        {
            RuleFor(d => d.FirebaseUid).NotNull().NotEmpty();
        }
    }
}
