using FieldServiceManagement.ViewModels.Company;

namespace FieldServiceManagement.ViewModels.Interfaces
{
    public interface IHasCompanies
    {
        IEnumerable<CompanyListItemViewModel>? Companies { get; set; }
    }
}