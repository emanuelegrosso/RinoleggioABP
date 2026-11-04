using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rinoleggio.Books;
using Rinoleggio.Authors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volo.Abp.Application.Dtos;
namespace Rinoleggio.Web.Pages.Books;
public class EditModalModel : RinoleggioPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }
    [BindProperty]
    public CreateUpdateBookDto Book { get; set; }
    public List<SelectListItem> Authors { get; set; } = new();
    private readonly IBookAppService _bookAppService;
    private readonly IAuthorAppService _authorAppService;
    public EditModalModel(IBookAppService bookAppService, IAuthorAppService authorAppService)
    {
        _bookAppService = bookAppService;
        _authorAppService = authorAppService;
    }
    public async Task OnGetAsync()
    {
        await LoadAuthorsAsync();
        var bookDto = await _bookAppService.GetAsync(Id);
        Book = ObjectMapper.Map<BookDto, CreateUpdateBookDto>(bookDto);
    }
    public async Task<IActionResult> OnPostAsync()
    {
        await _bookAppService.UpdateAsync(Id, Book);
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
