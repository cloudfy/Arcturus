using Arcturus.ResultObjects;

namespace $rootnamespace$;

public static class $safeitemname$
{
    // TODO: implement the command
    public record Command
        (string Name) : IRequest<Result<ResultValue>>;

    public class Handler : IRequestHandler<Command, Result<ResultValue>>
    {
        public async Task<Result<ResultValue>> Handle(
            Command command
            , CancellationToken cancellationToken)
        {
            // TODO: implement the handler here
            
            return new ResultValue();
        }
    }

    // TODO: implement the result here
    public record ResultValue;
}