namespace $rootnamespace$;

public static class $safeitemname$
{
    // TODO: implement the command
    public record Command
        (string Name) : IRequest<Result>;

    public class Handler : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(
            Command command
            , CancellationToken cancellationToken)
        {
            // TODO: implement the handler here
            
            return Task.FromResult(new Result());
        }
    }

    // TODO: implement the result here
    public record Result;
}