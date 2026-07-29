// Controllers/CacheController.cs
using FieldServiceManagement.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FieldServiceManagement.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class CacheController : Controller
    {

        public IActionResult Index()
        {
            return View("~/Views/Admin/FlushCache.cshtml");
        }

        // POST: /Cache/FlushAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FlushAll()
        {
            try
            {
                CacheBusiness.FlushAll();
                TempData["Success"] = "Entire cache flushed successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to flush cache: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cache/FlushRegion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FlushRegion(string region)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(region))
                {
                    TempData["Error"] = "Region is required.";
                    return RedirectToAction(nameof(Index));
                }

                CacheBusiness.FlushByRegion(region);
                TempData["Success"] = $"Region '{region}' flushed successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to flush region: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cache/FlushKey
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FlushKey(string key, string? region = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    TempData["Error"] = "Key is required.";
                    return RedirectToAction(nameof(Index));
                }

                CacheBusiness.FlushByKey(key, region);
                TempData["Success"] = $"Key '{key}' removed successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to remove key: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cache/RevalidateKey
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RevalidateKey(string key, string? region = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    TempData["Error"] = "Key is required.";
                    return RedirectToAction(nameof(Index));
                }

                CacheBusiness.Revalidate(key, region);
                TempData["Success"] = $"Key '{key}' marked for revalidation.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to revalidate key: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cache/RevalidateRegion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RevalidateRegion(string region)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(region))
                {
                    TempData["Error"] = "Region is required.";
                    return RedirectToAction(nameof(Index));
                }

                CacheBusiness.RevalidateRegion(region);
                TempData["Success"] = $"Region '{region}' marked for revalidation.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to revalidate region: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}