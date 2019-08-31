using Microsoft.AspNetCore.Http;

public interface IHandler
{
    public string id { get; set; }
    public Response Handler(HttpRequest req);
}