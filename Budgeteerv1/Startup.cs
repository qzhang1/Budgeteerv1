using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Budgeteerv1.Startup))]
namespace Budgeteerv1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
