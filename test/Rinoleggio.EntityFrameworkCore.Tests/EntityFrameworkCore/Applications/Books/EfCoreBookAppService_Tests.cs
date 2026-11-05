using Rinoleggio.Books;
using Xunit;

namespace Rinoleggio.EntityFrameworkCore.Applications.Books;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class EfCoreBookAppService_Tests : BookAppService_Tests<RinoleggioEntityFrameworkCoreTestModule>
{

}