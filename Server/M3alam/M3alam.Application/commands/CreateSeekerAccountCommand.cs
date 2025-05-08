public class CreateSeekerAccountCommand : IRequest<bool>
{
    public CreateSeekerAccountDto Data { get; }

    public CreateSeekerAccountCommand(CreateSeekerAccountDto data)
    {
        Data = data;
    }
}

public class CreateSeekerAccountCommandHandler : IRequestHandler<CreateSeekerAccountCommand, bool>
{
    private readonly ISeekerService _service;

    public CreateSeekerAccountCommandHandler(ISeekerService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(CreateSeekerAccountCommand request, CancellationToken cancellationToken)
    {
        return await _service.CreateSeekerAccount(request.Data);
    }
}
