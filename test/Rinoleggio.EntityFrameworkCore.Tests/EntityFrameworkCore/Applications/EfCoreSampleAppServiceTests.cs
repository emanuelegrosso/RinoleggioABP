using Rinoleggio.Samples;
using Xunit;

namespace Rinoleggio.EntityFrameworkCore.Applications;

[Collection(RinoleggioTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<RinoleggioEntityFrameworkCoreTestModule>
{

}
