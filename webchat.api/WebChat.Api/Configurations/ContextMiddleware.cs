using WebChat.Infra;

namespace WebChat.Api.Configurations;

public class ContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var dbContext = context.RequestServices.GetRequiredService<WebChatContext>();

        await next.Invoke(context);

        await dbContext.SaveChangesAsync();
    }
}
