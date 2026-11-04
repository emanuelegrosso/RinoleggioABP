using System.Threading.Tasks;
using Rinoleggio.Authors;
using Microsoft.AspNetCore.Mvc;
namespace Rinoleggio.Web.Pages.Authors
{
    public class CreateModalModel : RinoleggioPageModel
    {
        [BindProperty]
        public CreateUpdateAuthorDto Author { get; set; }
        private readonly IAuthorAppService _authorAppService;
        public CreateModalModel(IAuthorAppService authorAppService)
        {
            _authorAppService = authorAppService;
        }
        public void OnGet()
        {
            Author = new CreateUpdateAuthorDto();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            await _authorAppService.CreateAsync(Author);
            return NoContent();
        }
    }
}
