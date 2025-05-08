public class CreateProviderAccountCommand : IRequest<bool>
{
    public CreateProviderAccountDto Data { get; }

    public CreateProviderAccountCommand(CreateProviderAccountDto data)
    {
        Data = data;
    }
}

public class CreateProviderAccountCommandHandler : IRequestHandler<CreateProviderAccountCommand, bool>
{
    private readonly IProviderService _service;

    public CreateProviderAccountCommandHandler(IProviderService service)
    {
        _service = service;
    }

    public async Task<bool> Handle(CreateProviderAccountCommand request, CancellationToken cancellationToken)
    {
        return await _service.CreateProviderAccount(request.Data);
    }
}
