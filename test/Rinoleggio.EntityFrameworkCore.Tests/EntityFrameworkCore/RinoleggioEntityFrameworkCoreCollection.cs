using Xunit;

namespace Rinoleggio.EntityFrameworkCore;

[CollectionDefinition(RinoleggioTestConsts.CollectionDefinitionName)]
public class RinoleggioEntityFrameworkCoreCollection : ICollectionFixture<RinoleggioEntityFrameworkCoreFixture>
{

}
