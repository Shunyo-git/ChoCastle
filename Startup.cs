using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChoCastle.Startup))]
namespace ChoCastle
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
