using FieldServiceManagement.Business.AddressBusiness;
using FieldServiceManagement.Business.CountryBusiness;
using FieldServiceManagement.Business.CrewBusiness;
using FieldServiceManagement.Business.CurrencyBusiness;
using FieldServiceManagement.Business.IndustryBusiness;
using FieldServiceManagement.Business.LanguageBusiness;
using FieldServiceManagement.Business.StateBusiness;
using FieldServiceManagement.Business.TimezoneBusiness;
using FieldServiceManagement.Business.UserBusiness;
using FieldServiceManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using static FieldServiceManagement.Helpers.HtmlHelpers;

namespace FieldServiceManagement.Controllers
{
    public class SearchController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetIndustries(string SearchName)
        {
            var Industries = new IndustryBusiness().GetIndustryBySearchName(SanitizeInput(SearchName));
            return Json(Industries);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetIndustryCategories(string SearchName, int IndustryId)
        {
            var IndustryCategories = new IndustryBusiness().GetIndustryCategoryBySearchNameAndIndustryId(SanitizeInput(SearchName), IndustryId);
            return Json(IndustryCategories); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetCountries(string SearchName)
        {
            var Countrues = new CountryBusiness().GetCountriesBySearchName(SanitizeInput(SearchName));
            return Json(Countrues);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetStates(string SearchName, int CountryId)
        {
            var State = new StateBusiness().GetStateBySearchName(SanitizeInput(SearchName), CountryId);
            return Json(State);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetCurrencies(string SearchName)
        {
            var currency = new CurrencyBusiness().GetCurrencyBySearchName(SanitizeInput(SearchName));
            return Json(currency);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetTimezones(string SearchName)
        {
            var Timezone = new TimezoneBusiness().GetTimezoneBySearchName(SanitizeInput(SearchName));
            return Json(Timezone);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetLanguages(string SearchName)
        {
            var Languaes = new LanguageBusiness().GetLanguagesBySearchName(SanitizeInput(SearchName));
            return Json(Languaes);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetBillingAddress(string SearchName)
        {
            var BillingAddress = new AddressBusiness().GetAddressBySearchNameAndOrganisationIdAsync(SanitizeInput(SearchName), User?.Identity?.Name!);
            return Json(BillingAddress);
        }

        [HttpGet]
        public async Task<IActionResult> GetCrew(string term)
        {
            var organisationId = User.GetOrganisationIdOrThrow();
            var results = await new CrewBusiness().SearchCrewsAsync(SanitizeInput(term), organisationId);

            var payload = results.Select(c => new
            {
                id = c.Id,
                name = c.CrewSize.HasValue ? $"{c.Name} ({c.CrewSize} members)" : c.Name
            });

            return Json(payload);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser(string term)
        {
            var organisationId = User.GetOrganisationIdOrThrow();

            var results = await new UserBusiness().SearchUsersAsync(term, organisationId);

            var payload = results.Select(u => new
            {
                id = u.Id,
                name = $"{u.Name} {u.Surname}".Trim() + (string.IsNullOrWhiteSpace(u.Email) ? "" : $" ({u.Email})")
            });

            return Json(payload);
        }
    }
}
