using Microsoft.AspNetCore.Builder;
using Rinoleggio;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("Rinoleggio.Web.csproj"); 
await builder.RunAbpModuleAsync<RinoleggioWebTestModule>(applicationName: "Rinoleggio.Web");

public partial class Program
{
}
