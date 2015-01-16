namespace WebApplication2.Handlers
{
    using MediatR;

    public class PingHandler : IRequestHandler<Ping, PingResponse>
    {
        public PingResponse Handle(Ping message)
        {
            return new PingResponse {Message = "Craig"};
        }
    }
}