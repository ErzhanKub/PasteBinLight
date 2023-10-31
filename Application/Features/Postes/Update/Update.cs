using Application.Contracts;

namespace Application.Features.Postes.Update
{
    public class UpdatePosteByIdCommand : IRequest<Result<PosteDto>>
    {
        public Guid PosteId { get; set; }
        public required string Text { get; set; }
        public string? Title { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime DeadLine { get; set; }
    }
}
