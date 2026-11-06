using System;
using System.Linq;
using System.Threading.Tasks;
using Rinoleggio.Identity;
using Rinoleggio.Listings;
using Rinoleggio.Lookups;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Rinoleggio.Listings;

public class ListingDemoDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public static readonly Guid SeedMarkerListingId = Guid.Parse("a0000000-0000-0000-0000-000000000001");

    private readonly IRepository<Listing, Guid> _listingRepository;
    private readonly IRepository<RentalCompany, Guid> _rentalCompanyRepository;
    private readonly IRepository<Brand, Guid> _brandRepository;
    private readonly IRepository<Province, Guid> _provinceRepository;
    private readonly IClock _clock;

    public ListingDemoDataSeedContributor(
        IRepository<Listing, Guid> listingRepository,
        IRepository<RentalCompany, Guid> rentalCompanyRepository,
        IRepository<Brand, Guid> brandRepository,
        IRepository<Province, Guid> provinceRepository,
        IClock clock)
    {
        _listingRepository = listingRepository;
        _rentalCompanyRepository = rentalCompanyRepository;
        _brandRepository = brandRepository;
        _provinceRepository = provinceRepository;
        _clock = clock;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        var companies = await SeedRentalCompaniesAsync();
        var brands = await SeedBrandsAsync();
        var provinces = await SeedProvincesAsync();
        var ownerId = RinoleggioIdentityDataSeedContributor.DemoDealerUserId;
        var today = DateOnly.FromDateTime(_clock.Now);
        var now = _clock.Now;

        if (await _listingRepository.AnyAsync(l => l.Id == SeedMarkerListingId))
        {
            await SeedSpecialListingsAsync(ownerId, companies, brands, provinces, today, now);
            return;
        }

        var templates = new[]
        {
            (Title: "Fiat 500e Icon", Brand: 0, Model: "500e", Version: "Icon", Fuel: FuelType.Electric, Trans: Transmission.Automatic, Year: (short)2023, Company: 0, Fee: 349m, Vat: true, Init: 0m, TotalKm: 60000, CurrentKm: 12000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: true, Ins: true, Road: true),
            (Title: "Volkswagen Golf 1.5 TSI", Brand: 1, Model: "Golf", Version: "1.5 TSI Life", Fuel: FuelType.Petrol, Trans: Transmission.Manual, Year: (short)2022, Company: 1, Fee: 429m, Vat: false, Init: 2500m, TotalKm: 90000, CurrentKm: 41000, Contrib: TransferContributionType.Incentive, ContribAmt: 800m, Maint: true, Ins: false, Road: true),
            (Title: "Toyota Yaris Hybrid", Brand: 2, Model: "Yaris", Version: "Hybrid Active", Fuel: FuelType.MildHybrid, Trans: Transmission.Automatic, Year: (short)2024, Company: 2, Fee: 319m, Vat: true, Init: 0m, TotalKm: 45000, CurrentKm: 5000, Contrib: TransferContributionType.Compensation, ContribAmt: 500m, Maint: true, Ins: true, Road: false),
            (Title: "BMW Serie 1 118d", Brand: 3, Model: "Serie 1", Version: "118d Business", Fuel: FuelType.Diesel, Trans: Transmission.Automatic, Year: (short)2021, Company: 0, Fee: 499m, Vat: false, Init: 4000m, TotalKm: 120000, CurrentKm: 78000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: false, Ins: true, Road: true),
            (Title: "Renault Captur TCe", Brand: 4, Model: "Captur", Version: "TCe 130", Fuel: FuelType.Petrol, Trans: Transmission.Manual, Year: (short)2023, Company: 1, Fee: 379m, Vat: true, Init: 1500m, TotalKm: 60000, CurrentKm: 22000, Contrib: TransferContributionType.Incentive, ContribAmt: 300m, Maint: true, Ins: true, Road: true),
            (Title: "Peugeot 3008 Hybrid", Brand: 5, Model: "3008", Version: "Hybrid 225", Fuel: FuelType.PlugInHybrid, Trans: Transmission.Automatic, Year: (short)2023, Company: 2, Fee: 549m, Vat: true, Init: 0m, TotalKm: 75000, CurrentKm: 18000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: true, Ins: true, Road: true),
            (Title: "Audi A3 Sportback", Brand: 6, Model: "A3", Version: "Sportback 35 TFSI", Fuel: FuelType.Petrol, Trans: Transmission.Automatic, Year: (short)2022, Company: 0, Fee: 459m, Vat: false, Init: 3000m, TotalKm: 80000, CurrentKm: 35000, Contrib: TransferContributionType.Compensation, ContribAmt: 1200m, Maint: true, Ins: false, Road: false),
            (Title: "Mercedes Classe A", Brand: 7, Model: "Classe A", Version: "A180d", Fuel: FuelType.Diesel, Trans: Transmission.Automatic, Year: (short)2021, Company: 1, Fee: 519m, Vat: true, Init: 5000m, TotalKm: 100000, CurrentKm: 62000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: false, Ins: true, Road: true),
            (Title: "Ford Kuga PHEV", Brand: 8, Model: "Kuga", Version: "PHEV ST-Line", Fuel: FuelType.PlugInHybrid, Trans: Transmission.Automatic, Year: (short)2024, Company: 2, Fee: 489m, Vat: true, Init: 0m, TotalKm: 50000, CurrentKm: 8000, Contrib: TransferContributionType.Incentive, ContribAmt: 600m, Maint: true, Ins: true, Road: true),
            (Title: "Opel Corsa GPL", Brand: 9, Model: "Corsa", Version: "1.2 GPL", Fuel: FuelType.LPG, Trans: Transmission.Manual, Year: (short)2020, Company: 0, Fee: 279m, Vat: false, Init: 0m, TotalKm: 90000, CurrentKm: 55000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: true, Ins: false, Road: true),
            (Title: "Hyundai Kona Electric", Brand: 10, Model: "Kona", Version: "Electric 65kWh", Fuel: FuelType.Electric, Trans: Transmission.Automatic, Year: (short)2023, Company: 1, Fee: 399m, Vat: true, Init: 2000m, TotalKm: 60000, CurrentKm: 15000, Contrib: TransferContributionType.Compensation, ContribAmt: 400m, Maint: true, Ins: true, Road: true),
            (Title: "Kia Sportage Mild Hybrid", Brand: 11, Model: "Sportage", Version: "1.6 CRDi MHEV", Fuel: FuelType.MildHybrid, Trans: Transmission.Manual, Year: (short)2022, Company: 2, Fee: 419m, Vat: false, Init: 3500m, TotalKm: 85000, CurrentKm: 44000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: false, Ins: true, Road: false),
            (Title: "Citroen C3 PureTech", Brand: 12, Model: "C3", Version: "PureTech 110", Fuel: FuelType.Petrol, Trans: Transmission.Manual, Year: (short)2021, Company: 0, Fee: 299m, Vat: true, Init: 0m, TotalKm: 70000, CurrentKm: 39000, Contrib: TransferContributionType.Incentive, ContribAmt: 250m, Maint: true, Ins: true, Road: true),
            (Title: "Nissan Qashqai e-POWER", Brand: 13, Model: "Qashqai", Version: "e-POWER N-Connecta", Fuel: FuelType.MildHybrid, Trans: Transmission.Automatic, Year: (short)2024, Company: 1, Fee: 469m, Vat: true, Init: 1000m, TotalKm: 45000, CurrentKm: 6000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: true, Ins: true, Road: true),
            (Title: "Seat Leon TDI", Brand: 14, Model: "Leon", Version: "2.0 TDI Style", Fuel: FuelType.Diesel, Trans: Transmission.Manual, Year: (short)2020, Company: 2, Fee: 359m, Vat: false, Init: 0m, TotalKm: 110000, CurrentKm: 88000, Contrib: TransferContributionType.Compensation, ContribAmt: 900m, Maint: false, Ins: false, Road: true),
            (Title: "Skoda Octavia CNG", Brand: 15, Model: "Octavia", Version: "1.5 TGI", Fuel: FuelType.CNG, Trans: Transmission.Manual, Year: (short)2019, Company: 0, Fee: 329m, Vat: true, Init: 0m, TotalKm: 130000, CurrentKm: 95000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: true, Ins: false, Road: false),
            (Title: "Alfa Romeo Tonale", Brand: 16, Model: "Tonale", Version: "1.6 Diesel", Fuel: FuelType.Diesel, Trans: Transmission.Automatic, Year: (short)2023, Company: 1, Fee: 529m, Vat: true, Init: 4500m, TotalKm: 60000, CurrentKm: 21000, Contrib: TransferContributionType.Incentive, ContribAmt: 700m, Maint: true, Ins: true, Road: true),
            (Title: "Jeep Compass 4xe", Brand: 17, Model: "Compass", Version: "4xe Limited", Fuel: FuelType.PlugInHybrid, Trans: Transmission.Automatic, Year: (short)2022, Company: 2, Fee: 479m, Vat: false, Init: 2000m, TotalKm: 75000, CurrentKm: 33000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: true, Ins: true, Road: true),
            (Title: "Mini Cooper SE", Brand: 18, Model: "Cooper", Version: "SE Electric", Fuel: FuelType.Electric, Trans: Transmission.Automatic, Year: (short)2024, Company: 0, Fee: 419m, Vat: true, Init: 0m, TotalKm: 40000, CurrentKm: 3000, Contrib: TransferContributionType.Compensation, ContribAmt: 350m, Maint: true, Ins: true, Road: true),
            (Title: "Dacia Sandero Stepway", Brand: 19, Model: "Sandero", Version: "Stepway TCe", Fuel: FuelType.Petrol, Trans: Transmission.Manual, Year: (short)2023, Company: 1, Fee: 249m, Vat: true, Init: 0m, TotalKm: 60000, CurrentKm: 17000, Contrib: TransferContributionType.None, ContribAmt: 0m, Maint: false, Ins: false, Road: true)
        };

        for (var i = 0; i < templates.Length; i++)
        {
            var t = templates[i];
            var id = Guid.Parse($"a0000000-0000-0000-0000-{((i + 1).ToString("D12"))}");
            var endDate = today.AddMonths(6 + (i % 18));
            var listing = new Listing(id)
            {
                OwnerId = ownerId,
                Title = t.Title,
                DescriptionHtml = $"<p>Annuncio dimostrativo per {t.Title}.</p>",
                SlugPath = $"annunci/demo-{i + 1}-{id:N}",
                ProvinceId = provinces[i % provinces.Count].Id,
                TransferContributionType = t.Contrib,
                TransferContributionAmount = t.ContribAmt,
                Contract = new ListingContract
                {
                    ListingId = id,
                    RentalCompanyId = companies[t.Company].Id,
                    StartDate = today.AddYears(-2),
                    EndDate = endDate,
                    MonthlyFee = t.Fee,
                    VatIncluded = t.Vat,
                    InitialPayment = t.Init,
                    TotalKm = t.TotalKm,
                    CurrentKm = t.CurrentKm,
                    RemainingKm = ListingKmCalculator.CalculateRemainingKm(t.TotalKm, t.CurrentKm),
                    ExcessKmRate = 0.12m + (i % 5) * 0.01m,
                    UnderKmRate = 0.05m
                },
                Vehicle = new ListingVehicle
                {
                    ListingId = id,
                    BrandId = brands[t.Brand].Id,
                    Model = t.Model,
                    Version = t.Version,
                    FuelType = t.Fuel,
                    Transmission = t.Trans,
                    RegistrationYear = t.Year
                },
                Services = new ListingServices
                {
                    ListingId = id,
                    MaintenanceIncluded = t.Maint,
                    InsuranceIncluded = t.Ins,
                    RoadsideAssistanceIncluded = t.Road,
                    ReplacementCarIncluded = i % 3 == 0,
                    TiresIncluded = i % 2 == 0,
                    RoadTaxIncluded = i % 4 == 0
                }
            };

            if (t.Ins)
            {
                listing.Services.InsuranceTypes.Add(new ListingInsuranceType { ListingId = id, InsuranceKind = InsuranceKind.RCA });
                if (i % 2 == 0)
                {
                    listing.Services.InsuranceTypes.Add(new ListingInsuranceType { ListingId = id, InsuranceKind = InsuranceKind.Kasko });
                }

                if (i % 5 == 0)
                {
                    listing.Services.InsuranceTypes.Add(new ListingInsuranceType { ListingId = id, InsuranceKind = InsuranceKind.Cristalli });
                }
            }

            listing.ValidateTransferContribution();
            listing.Publish(now, today);
            await _listingRepository.InsertAsync(listing, autoSave: true);
        }

        await SeedSpecialListingsAsync(ownerId, companies, brands, provinces, today, now);
    }

    private async Task SeedSpecialListingsAsync(
        Guid ownerId,
        System.Collections.Generic.List<RentalCompany> companies,
        System.Collections.Generic.List<Brand> brands,
        System.Collections.Generic.List<Province> provinces,
        DateOnly today,
        DateTime now)
    {
        var draftId = Guid.Parse("a0000000-0000-0000-0000-000000000099");
        if (!await _listingRepository.AnyAsync(l => l.Id == draftId))
        {
            var draft = BuildListing(draftId, ownerId, "Bozza dimostrativa", companies[0], brands[0], provinces[0], today, now);
            draft.SaveAsDraft();
            await _listingRepository.InsertAsync(draft, autoSave: true);
        }

        var reservedId = Guid.Parse("a0000000-0000-0000-0000-000000000098");
        if (!await _listingRepository.AnyAsync(l => l.Id == reservedId))
        {
            var reserved = BuildListing(reservedId, ownerId, "Annuncio riservato demo", companies[1], brands[1], provinces[1], today, now);
            reserved.Publish(now, today);
            reserved.MarkReserved(StatusChangedBy.Seller);
            await _listingRepository.InsertAsync(reserved, autoSave: true);
        }

        var suspendedId = Guid.Parse("a0000000-0000-0000-0000-000000000097");
        if (!await _listingRepository.AnyAsync(l => l.Id == suspendedId))
        {
            var suspended = BuildListing(suspendedId, ownerId, "Annuncio sospeso demo", companies[2], brands[2], provinces[2], today, now);
            suspended.Publish(now, today);
            suspended.Suspend("Controllo documenti", StatusChangedBy.BackOffice);
            await _listingRepository.InsertAsync(suspended, autoSave: true);
        }

        var withdrawnId = Guid.Parse("a0000000-0000-0000-0000-000000000096");
        if (!await _listingRepository.AnyAsync(l => l.Id == withdrawnId))
        {
            var withdrawn = BuildListing(withdrawnId, ownerId, "Annuncio ritirato demo", companies[3], brands[3], provinces[3], today, now);
            withdrawn.Publish(now, today);
            withdrawn.Withdraw("Ritirato dal venditore", StatusChangedBy.Seller);
            await _listingRepository.InsertAsync(withdrawn, autoSave: true);
        }
    }

    private static Listing BuildListing(
        Guid id,
        Guid ownerId,
        string title,
        RentalCompany company,
        Brand brand,
        Province province,
        DateOnly today,
        DateTime now)
    {
        var listing = new Listing(id)
        {
            OwnerId = ownerId,
            Title = title,
            DescriptionHtml = $"<p>{title}</p>",
            SlugPath = $"annunci/special-{id:N}",
            ProvinceId = province.Id,
            Contract = new ListingContract
            {
                ListingId = id,
                RentalCompanyId = company.Id,
                StartDate = today.AddYears(-1),
                EndDate = today.AddMonths(10),
                MonthlyFee = 399m,
                VatIncluded = true,
                InitialPayment = 0,
                TotalKm = 60000,
                CurrentKm = 15000,
                RemainingKm = 45000
            },
            Vehicle = new ListingVehicle
            {
                ListingId = id,
                BrandId = brand.Id,
                Model = "Demo",
                Version = "Special",
                FuelType = FuelType.Petrol,
                Transmission = Transmission.Automatic,
                RegistrationYear = (short)today.Year
            },
            Services = new ListingServices { ListingId = id, MaintenanceIncluded = true }
        };
        listing.ValidateTransferContribution();
        return listing;
    }

    private async Task<System.Collections.Generic.List<RentalCompany>> SeedRentalCompaniesAsync()
    {
        var names = new[] { "Leasys", "Arval", "Alphabet", "LeasePlan" };
        var result = new System.Collections.Generic.List<RentalCompany>();
        for (var i = 0; i < names.Length; i++)
        {
            var id = Guid.Parse($"b0000000-0000-0000-0000-{((i + 1).ToString("D12"))}");
            var existing = await _rentalCompanyRepository.FindAsync(id);
            if (existing != null)
            {
                result.Add(existing);
                continue;
            }

            var entity = new RentalCompany(id) { Name = names[i], IsActive = true };
            await _rentalCompanyRepository.InsertAsync(entity, autoSave: true);
            result.Add(entity);
        }

        return result;
    }

    private async Task<System.Collections.Generic.List<Brand>> SeedBrandsAsync()
    {
        var names = new[]
        {
            "Fiat", "Volkswagen", "Toyota", "BMW", "Renault", "Peugeot", "Audi", "Mercedes-Benz",
            "Ford", "Opel", "Hyundai", "Kia", "Citroen", "Nissan", "Seat", "Skoda", "Alfa Romeo",
            "Jeep", "Mini", "Dacia",
            "Abarth", "Aiways", "Alpine", "Aston Martin", "Bentley", "BYD", "Cadillac", "Chery",
            "Chevrolet", "Chrysler", "Cupra", "Daewoo", "Daihatsu", "Dodge", "DR", "DS Automobiles",
            "EVO", "Ferrari", "Fisker", "Genesis", "Honda", "Hummer", "INEOS", "Infiniti", "Isuzu",
            "Jaguar", "KGM", "Lamborghini", "Lancia", "Land Rover", "Leapmotor", "Lexus", "Lotus",
            "Lucid", "Lynk & Co", "Mahindra", "Maserati", "Mazda", "McLaren", "MG", "Mitsubishi",
            "NIO", "ORA", "Pagani", "Polestar", "Porsche", "RAM", "Rivian", "Rolls-Royce", "Saab",
            "Smart", "Subaru", "Suzuki", "Tata", "Tesla", "VinFast", "Volvo", "XPENG", "ZEEKR"
        };
        var result = new System.Collections.Generic.List<Brand>();
        for (var i = 0; i < names.Length; i++)
        {
            var id = Guid.Parse($"c0000000-0000-0000-0000-{((i + 1).ToString("D12"))}");
            var existing = await _brandRepository.FindAsync(id) ??
                           await _brandRepository.FirstOrDefaultAsync(x => x.Name == names[i]);
            if (existing != null)
            {
                result.Add(existing);
                continue;
            }

            var entity = new Brand(id) { Name = names[i], SortOrder = i, IsActive = true };
            await _brandRepository.InsertAsync(entity, autoSave: true);
            result.Add(entity);
        }

        return result;
    }

    private async Task<System.Collections.Generic.List<Province>> SeedProvincesAsync()
    {
        // Keep the original five entries first: the demo listings use their indexes.
        // Province sigles follow the current Italian territorial units, including the 2026 Sardinia reorganization.
        var data = new[]
        {
            ("Milano", "MI"), ("Roma", "RM"), ("Torino", "TO"), ("Bologna", "BO"), ("Napoli", "NA"),
            ("Aosta", "AO"), ("Alessandria", "AL"), ("Ancona", "AN"), ("Arezzo", "AR"), ("Agrigento", "AG"),
            ("Ascoli Piceno", "AP"), ("Asti", "AT"), ("Avellino", "AV"), ("Bari", "BA"), ("Barletta-Andria-Trani", "BT"),
            ("Belluno", "BL"), ("Benevento", "BN"), ("Bergamo", "BG"), ("Biella", "BI"), ("Bolzano", "BZ"),
            ("Brescia", "BS"), ("Brindisi", "BR"), ("Cagliari", "CA"), ("Caltanissetta", "CL"), ("Campobasso", "CB"),
            ("Caserta", "CE"), ("Catania", "CT"), ("Catanzaro", "CZ"), ("Chieti", "CH"), ("Como", "CO"),
            ("Cosenza", "CS"), ("Cremona", "CR"), ("Crotone", "KR"), ("Cuneo", "CN"), ("Enna", "EN"),
            ("Fermo", "FM"), ("Ferrara", "FE"), ("Firenze", "FI"), ("Foggia", "FG"), ("Forlì-Cesena", "FC"),
            ("Frosinone", "FR"), ("Genova", "GE"), ("Gorizia", "GO"), ("Grosseto", "GR"), ("Imperia", "IM"),
            ("Isernia", "IS"), ("L'Aquila", "AQ"), ("La Spezia", "SP"), ("Latina", "LT"), ("Lecce", "LE"), ("Lecco", "LC"),
            ("Livorno", "LI"), ("Lodi", "LO"), ("Lucca", "LU"), ("Macerata", "MC"), ("Mantova", "MN"),
            ("Massa-Carrara", "MS"), ("Matera", "MT"), ("Messina", "ME"), ("Modena", "MO"), ("Monza e della Brianza", "MB"),
            ("Nuoro", "NU"), ("Novara", "NO"), ("Oristano", "OR"), ("Padova", "PD"), ("Palermo", "PA"),
            ("Parma", "PR"), ("Pavia", "PV"), ("Perugia", "PG"), ("Pesaro e Urbino", "PU"), ("Pescara", "PE"),
            ("Piacenza", "PC"), ("Pisa", "PI"), ("Pistoia", "PT"), ("Pordenone", "PN"), ("Potenza", "PZ"),
            ("Prato", "PO"), ("Ragusa", "RG"), ("Ravenna", "RA"), ("Reggio Calabria", "RC"), ("Reggio Emilia", "RE"),
            ("Rieti", "RI"), ("Rimini", "RN"), ("Rovigo", "RO"), ("Salerno", "SA"), ("Sassari", "SS"),
            ("Savona", "SV"), ("Siena", "SI"), ("Siracusa", "SR"), ("Sondrio", "SO"), ("Taranto", "TA"),
            ("Teramo", "TE"), ("Terni", "TR"), ("Trapani", "TP"), ("Trento", "TN"), ("Treviso", "TV"),
            ("Trieste", "TS"), ("Udine", "UD"), ("Varese", "VA"), ("Venezia", "VE"), ("Verbano-Cusio-Ossola", "VB"),
            ("Vercelli", "VC"), ("Verona", "VR"), ("Vibo Valentia", "VV"), ("Vicenza", "VI"), ("Viterbo", "VT"),
            ("Gallura Nord-Est Sardegna", "OT"), ("Ogliastra", "OG"), ("Medio Campidano", "VS"), ("Sulcis Iglesiente", "SU")
        };
        var result = new System.Collections.Generic.List<Province>();
        for (var i = 0; i < data.Length; i++)
        {
            var id = Guid.Parse($"d0000000-0000-0000-0000-{((i + 1).ToString("D12"))}");
            var existing = await _provinceRepository.FindAsync(id) ??
                           await _provinceRepository.FirstOrDefaultAsync(x => x.Code == data[i].Item2);
            if (existing != null)
            {
                result.Add(existing);
                continue;
            }

            var entity = new Province(id)
            {
                Name = data[i].Item1,
                Code = data[i].Item2,
                SortOrder = i,
                IsActive = true
            };
            await _provinceRepository.InsertAsync(entity, autoSave: true);
            result.Add(entity);
        }

        return result;
    }
}
