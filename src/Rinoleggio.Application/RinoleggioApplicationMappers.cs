using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;
using Rinoleggio.Authors;
using Rinoleggio.Books;
using Rinoleggio.Lookups;

namespace Rinoleggio;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioBookToBookDtoMapper : MapperBase<Book, BookDto>
{
    [MapperIgnoreTarget(nameof(BookDto.AuthorName))]
    public override partial BookDto Map(Book source);

    [MapperIgnoreTarget(nameof(BookDto.AuthorName))]
    public override partial void Map(Book source, BookDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioCreateUpdateBookDtoToBookMapper : MapperBase<CreateUpdateBookDto, Book>
{
    public override partial Book Map(CreateUpdateBookDto source);

    public override partial void Map(CreateUpdateBookDto source, Book destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioAuthorToAuthorDtoMapper : MapperBase<Author, AuthorDto>
{
    public override partial AuthorDto Map(Author source);

    public override partial void Map(Author source, AuthorDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioCreateUpdateAuthorDtoToAuthorMapper : MapperBase<CreateUpdateAuthorDto, Author>
{
    public override partial Author Map(CreateUpdateAuthorDto source);

    public override partial void Map(CreateUpdateAuthorDto source, Author destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioAuthorToAuthorExcelDtoMapper : MapperBase<Author, AuthorExcelDto>
{
    public override partial AuthorExcelDto Map(Author source);

    public override partial void Map(Author source, AuthorExcelDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioRentalCompanyMapper : MapperBase<RentalCompany, RentalCompanyDto>
{
    public override partial RentalCompanyDto Map(RentalCompany source);
    public override partial void Map(RentalCompany source, RentalCompanyDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioCreateUpdateRentalCompanyMapper : MapperBase<CreateUpdateRentalCompanyDto, RentalCompany>
{
    public override partial RentalCompany Map(CreateUpdateRentalCompanyDto source);
    public override partial void Map(CreateUpdateRentalCompanyDto source, RentalCompany destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioBrandMapper : MapperBase<Brand, BrandDto>
{
    public override partial BrandDto Map(Brand source);
    public override partial void Map(Brand source, BrandDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioCreateUpdateBrandMapper : MapperBase<CreateUpdateBrandDto, Brand>
{
    public override partial Brand Map(CreateUpdateBrandDto source);
    public override partial void Map(CreateUpdateBrandDto source, Brand destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioProvinceMapper : MapperBase<Province, ProvinceDto>
{
    public override partial ProvinceDto Map(Province source);
    public override partial void Map(Province source, ProvinceDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class RinoleggioCreateUpdateProvinceMapper : MapperBase<CreateUpdateProvinceDto, Province>
{
    public override partial Province Map(CreateUpdateProvinceDto source);
    public override partial void Map(CreateUpdateProvinceDto source, Province destination);
}
