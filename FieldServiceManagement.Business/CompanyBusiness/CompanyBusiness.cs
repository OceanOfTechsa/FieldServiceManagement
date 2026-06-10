using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Company;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Company;

namespace FieldServiceManagement.Business.CompanyBusiness
{
    public class CompanyBusiness
    {
        public async Task<Guid?> CreateCompanyAsync(CreateCompanyViewModel model, string email)
        {
            var currentUser = await new UserBusiness.UserBusiness().GetUserDetailsByUserNameAsync(email);
            model.OrganisationId = currentUser.OrganisationId;
            model.CreatedBy = currentUser.Id;
            model.UpdatedBy = currentUser.Id;
    
            var company = ObjectMapper.Mapper.Map<Company>(model);
            return await new CompanyRepository().CreateCompanyAsync(company, new LocalIPResolver().GetRoutedIPv4());
        }

        public async Task<List<CompanyDetailsViewModel?>> GetCompanyFullDetailsByIdAsync(Guid id)
        {
            var result = await new CompanyRepository().GetCompanyFullDetailsByIdAsync(id);
            return ObjectMapper.Mapper.Map<List<CompanyDetailsViewModel?>>(result);
        }

        public async Task<IEnumerable<CompanyListItemViewModel>> GetCompaniesByUserEmailAsync(string Email)
        {
            var results = await new CompanyRepository().GetCompaniesByUserEmailAsync(Email);
            return ObjectMapper.Mapper.Map<List<CompanyListItemViewModel>>(results);
        }

        public async Task<CompanyViewModel?> GetCompanyByIdAsync(Guid id)
        {
            var result = await new CompanyRepository().GetCompanyByIdAsync(id);
            return ObjectMapper.Mapper.Map<CompanyViewModel?>(result);
        }

        public async Task<Guid?> UpdateCompanyAsync(CompanyViewModel model, string currentUserEmail)
        {
            var dbModel = ObjectMapper.Mapper.Map<Company>(model);
            dbModel.Id = model.Id;
            return await new CompanyRepository().UpdateCompanyAsync(dbModel, currentUserEmail, new LocalIPResolver().GetRoutedIPv4()!);
        }
    }
}
