using AutoMapper;
using FieldServiceManagement.Data.DataModels.DataImportHistory;
using FieldServiceManagement.ViewModels.DataImport;

namespace FieldServiceManagement.Business.MappingBusiness.Profiles
{
    public class DataImportProfile: Profile
    {
        public DataImportProfile()
        {
            CreateMap<DataImportHistory, DataImportHistoryViewModel>().ReverseMap();
        }
    }
}
