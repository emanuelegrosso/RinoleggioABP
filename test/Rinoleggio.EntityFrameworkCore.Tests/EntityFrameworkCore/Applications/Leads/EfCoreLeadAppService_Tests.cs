using Rinoleggio.Leads;
using Xunit;

namespace Rinoleggio.EntityFrameworkCore.Applications.Leads;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class EfCoreLeadAppService_Tests : LeadAppService_Tests<RinoleggioEntityFrameworkCoreTestModule>
{
}
