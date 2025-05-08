public class CreateRatingCommand : IRequest<RatingDto>
{
    public Guid SeekerId { get; }
    public CreateRatingDto CreateDto { get; }

    public CreateRatingCommand(Guid seekerId, CreateRatingDto createDto)
    {
        SeekerId = seekerId;
        CreateDto = createDto;
    }
}

public class CreateRatingCommandHandler : IRequestHandler<CreateRatingCommand, RatingDto>
{
    private readonly IRatingService _service;

    public CreateRatingCommandHandler(IRatingService service)
    {
        _service = service;
    }

    public async Task<RatingDto> Handle(CreateRatingCommand request, CancellationToken cancellationToken)
    {
        return await _service.CreateRatingAsync(request.SeekerId, request.CreateDto);
    }
}
