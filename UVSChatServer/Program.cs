using UVSChatServer.Components;
using UVSChatServer.Hubs;
using UVSMedia.Media;

namespace UVSChatServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddSignalR();

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(5000);
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseRouting();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.MapHub<ChatHub>("/chat");

            app.Run();
        }
    }
}
