using Rinoleggio.Samples;
using Xunit;

namespace Rinoleggio.EntityFrameworkCore.Domains;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<RinoleggioEntityFrameworkCoreTestModule>
{

}
