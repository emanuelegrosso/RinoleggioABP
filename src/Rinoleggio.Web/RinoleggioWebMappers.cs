using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using Rinoleggio.Authors;
using Rinoleggio.Books;
namespace Rinoleggio.Web;
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioWebMappers : MapperBase<BookDto, CreateUpdateBookDto>
{
    public override partial CreateUpdateBookDto Map(BookDto source);
    public override partial void Map(BookDto source, CreateUpdateBookDto destination);
}
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioAuthorDtoToCreateUpdateAuthorDtoMapper : MapperBase<AuthorDto, CreateUpdateAuthorDto>
{
    public override partial CreateUpdateAuthorDto Map(AuthorDto source);
    public override partial void Map(AuthorDto source, CreateUpdateAuthorDto destination);
}
