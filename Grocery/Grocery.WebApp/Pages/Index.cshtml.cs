using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty(SupportsGet=true)] // can write to id value by post or get (write through url)
        //[BindProperty] // use this if the want to write value to Id 
        //write to this id by post to this data [need form, hit submit to post data, post only]
        // use @Model.Id in Index.cshtml will pull this value
        public int Id { get; set; }



        // show/display page
        public void OnGet() // when you have a get request to this page, execute this
        {

        }
        
        // capture info off page (needed in signup page)
        public void OnPost() //execute whenever user posts data to this page
        {

        }
    }
}

