using WebChat.Infra;

namespace WebChat.API.Configurations;

public class ContextMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var dbContext = context.RequestServices.GetRequiredService<WebChatContext>();

        await next.Invoke(context);

        await dbContext.SaveChangesAsync();
    }
}
