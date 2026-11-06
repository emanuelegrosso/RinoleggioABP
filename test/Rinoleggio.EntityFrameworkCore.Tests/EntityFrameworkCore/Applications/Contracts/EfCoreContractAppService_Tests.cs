using Rinoleggio.Contracts;
using Xunit;

namespace Rinoleggio.EntityFrameworkCore.Applications.Contracts;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class EfCoreContractAppService_Tests : ContractAppService_Tests<RinoleggioEntityFrameworkCoreTestModule>
{
}

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class EfCoreContractAppService_Manage_Tests : ContractAppService_Manage_Tests<RinoleggioEntityFrameworkCoreTestModule>
{
}
