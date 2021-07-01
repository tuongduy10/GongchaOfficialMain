using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookStoreEntity.Startup))]
namespace BookStoreEntity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
