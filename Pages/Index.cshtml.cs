using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

using static System.Net.WebRequestMethods;

namespace WeatherApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }

        //Properties that API will return to user
        [BindProperty]
        public String Country { get; set; }
        [BindProperty]
        public String City { get; set; }
        [BindProperty]
        public String Temp { get; set; }
        [BindProperty]
        public String Description { get; set; }
        [BindProperty]
        public String Icon { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (String.IsNullOrEmpty(City))
            {
                ErrorMessage = "Please enter a city!";
                return Page();
            }

            string apiKey = "24e8015d53455d1f1ae07f78d3512b26";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={City}&units=metric&appid={apiKey}";

            try
            {
                using(HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if(!response.IsSuccessStatusCode)
                    {
                        ErrorMessage = "Error fetching data. Please input a correct city name!";
                        return Page();
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);

                    Country = data.sys.country;
                    Temp = data.main.temp.ToString();
                    Description = data.weather[0].description;
                    string icon_code = data.weather[0].icon;
                    Icon = $"http://openweathermap.org/img/wn/{icon_code}.png";
                    
                }
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Network error. Please try again later.";
            }

            return Page();
        }
    }
}
