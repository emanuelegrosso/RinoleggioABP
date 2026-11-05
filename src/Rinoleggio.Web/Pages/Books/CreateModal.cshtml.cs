using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rinoleggio.Books;
using Rinoleggio.Authors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
namespace Rinoleggio.Web.Pages.Books
{
    public class CreateModalModel : RinoleggioPageModel
    {
    [BindProperty]
    public CreateUpdateBookDto Book { get; set; }
    public List<SelectListItem> Authors { get; set; } = new();
    private readonly IBookAppService _bookAppService;
    private readonly IAuthorAppService _authorAppService;
    public CreateModalModel(IBookAppService bookAppService, IAuthorAppService authorAppService)
    {
        _bookAppService = bookAppService;
        _authorAppService = authorAppService;
    }
    public async Task OnGetAsync()
    {
        await LoadAuthorsAsync();
        Book = new CreateUpdateBookDto
        {
            AuthorId = Authors.FirstOrDefault()?.Value is { } authorId ? Guid.Parse(authorId) : Guid.Empty
        };
    }
    public async Task<IActionResult> OnPostAsync()
    {
        await _bookAppService.CreateAsync(Book);
        return NoContent();
    }
    private async Task LoadAuthorsAsync()
    {
        var authors = await _authorAppService.GetListAsync(new PagedAndSortedResultRequestDto
        {
            MaxResultCount = 1000,
            Sorting = nameof(AuthorDto.Name)
        });
        Authors = authors.Items.Select(author => new SelectListItem(author.Name, author.Id.ToString())).ToList();
    }
    }
}
