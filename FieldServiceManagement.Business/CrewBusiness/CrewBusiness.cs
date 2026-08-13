using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Crew;
using FieldServiceManagement.Data.DataModels.Organisation;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Audit;
using FieldServiceManagement.ViewModels.Crew;
using FieldServiceManagement.ViewModels.User;

namespace FieldServiceManagement.Business.CrewBusiness
{
    public class CrewBusiness
    {
        public async Task<IEnumerable<CrewListItemViewModel>> GetAllOrganisationCrews(Guid OrgId)
        {
            var results = await new CrewRepository().GetCrewsByOrganisationIdAsync(OrgId);
            return ObjectMapper.Mapper.Map<List<CrewListItemViewModel>>(results);
        }

        public async Task<BusinessResult> CreateCrew(CreateCrewViewModel model, string Email)
        {
            var currentUser = await new UserBusiness.UserBusiness().GetAllUserDetailsByUsernameAsync(Email);
            var result = await new SubscriptionPlanBusiness().ApplySubscriptionPlanRules(currentUser, SubscriptionRuleContext.CreateCrew);
            if (!result.Success) return result;

            var dbModel = BuildCreateCrewModel(model, currentUser);
            var newGuid = await new CrewRepository().CreateCrewAsync(dbModel);
            if (!newGuid.HasValue)
                return BusinessResult.Fail("Something went wrong whie creating the crew, please try again.");
            return BusinessResult.Ok();
        }

        public async Task<CrewFullProfileViewModel> GetCrewDetailsByIdAsync(Guid Id, Guid OrgId)
        {
            var result = await new CrewRepository().GetCrewByIdAsync(Id, OrgId);
            var crewRow = result.CrewRows.FirstOrDefault();
            if (crewRow == null)
                return new CrewFullProfileViewModel();

            var model = BuildCreDetailsModel(crewRow, result); 
            return model;
        }

        public async Task<CrewViewModel> GetCrewByIdAsync(Guid Id, Guid? OrgId)
        {
            var crew = await new CrewRepository().GetCrewByIdAsync(Id, OrgId);
            return ObjectMapper.Mapper.Map<CrewViewModel>(crew);
        }

        public async Task<List<CrewSearchResultViewModel>> SearchCrewsAsync(string term, Guid organisationId)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                return new List<CrewSearchResultViewModel>();
            
            var results = await new CrewRepository().SearchCrewsAsync(term.Trim(), organisationId);
            return ObjectMapper.Mapper.Map<List<CrewSearchResultViewModel>>(results);
        }

        public async Task<BusinessResult> AddCrewMemberAsync(Guid crewId, Guid userId, Guid organisationId, Guid performedBy, bool isLead = false)
        {
            var crewUsersLimit = await new CrewRepository().GetCrewUsersLimitAsync(crewId, organisationId);
            var NumberOfUsersInACrew = await new CrewRepository().GetNumberOfUsersInACrew(crewId, organisationId);
            
            if (NumberOfUsersInACrew >= crewUsersLimit)
                return BusinessResult.Fail($"This crew has reached its maximum member limit ({crewUsersLimit}). Manage members in Crew Settings.");
            
            var repoResult = await new CrewRepository().AddCrewMemberAsync(crewId, userId, organisationId, performedBy, isLead);
            return repoResult.Success ? BusinessResult.Ok() : BusinessResult.Fail("Failed to add crew member.");
        }

        public async Task<BusinessResult> ChangeLeadAsync(Guid CrewId, Guid UserId, Guid organisationId, string modifiedBy)
        {
            var result = await new CrewRepository().ChangeCrewLeadAsync(CrewId, UserId, organisationId, modifiedBy);
            if (!result.Success)
                return BusinessResult.Fail("Something went wrong while changing the crew lead, please try again.");
            return BusinessResult.Ok();
        }

        public async Task<BusinessResult> DeleteCrewAsync(Guid CrewId, Guid OrgId)
        {
            var result = await new CrewRepository().DeleteCrewAsync(CrewId, OrgId);
            if (!result.Success)
                return BusinessResult.Fail("Something went wrong while deleting the crew, please try again.");
            return BusinessResult.Ok();
        }

        public async Task<BusinessResult> RemoveMemberAsync(Guid CrewId, Guid UserId, Guid OrgId, string Email)
        {
            var result = await new CrewRepository().RemoveCrewMemberAsync(CrewId, UserId, OrgId, Email);
            if (!result.Success)
                return BusinessResult.Fail("Failed to remember crew member.");
            return BusinessResult.Ok();
        }

        public async Task<BusinessResult> UpdateCrewAsync(CrewFullProfileViewModel Model, Guid OrgId, string Email)
        {
            var result = await new CrewRepository().UpdateCrewAsync(Model.Id, Model.Name, Model.CrewSize, Model.Description, Model.IsActive, OrgId, Email);
            if (!result.Success)
                return BusinessResult.Fail("Failed to update crew");
            return BusinessResult.Ok();
        }

        #region PRIVATE METHODS
        private CreateCrew BuildCreateCrewModel(CreateCrewViewModel model, AppUserProfileViewModel currentUser)
        {
            var dbModel = ObjectMapper.Mapper.Map<CreateCrew>(model);
            dbModel.CreatedById = currentUser.User.Id;
            dbModel.OrganisationId = currentUser.Organisation!.Id;
            return dbModel;
        }

        private CrewFullProfileViewModel BuildCreDetailsModel(CrewDetails crewRow, CrewFullProfileResult result)
        {
            var model = new CrewFullProfileViewModel
            {
                Id = crewRow.Id,
                Name = crewRow.Name,
                CrewSize = crewRow.CrewSize,
                Description = crewRow.Description,
                CreatedAt = crewRow.CreatedAt,
                UpdatedAt = crewRow.UpdatedAt,
                IsActive = crewRow.IsActive,
                IsDeleted = crewRow.IsDeleted,
                OrganisationId = crewRow.OrganisationId,

                CB_Id = crewRow.CB_Id,
                CB_Name = crewRow.CB_Name,
                CB_Surname = crewRow.CB_Surname,
                CB_Email = crewRow.CB_Email,
                CB_AvatarUrl = crewRow.CB_AvatarUrl,
                CB_IsActive = crewRow.CB_IsActive,

                UB_Id = crewRow.UB_Id,
                UB_Name = crewRow.UB_Name,
                UB_Surname = crewRow.UB_Surname,
                UB_Email = crewRow.UB_Email,
                UB_AvatarUrl = crewRow.UB_AvatarUrl,
                UB_IsActive = crewRow.UB_IsActive,

                AuditLogs = GetAuditLogs(result),
                Members = GetCrewMembers(result)
            };

            return model;
        }

        private List<CrewMemberViewModel> GetCrewMembers(CrewFullProfileResult result)
        {
            var members = result.Members
            .Select(m => new CrewMemberViewModel
            {
                MembershipId = m.MembershipId,
                IsLead = m.IsLead,
                JoinedAt = m.JoinedAt,
                UserId = m.UserId,
                Name = m.Name ?? string.Empty,
                Surname = m.Surname,
                Email = m.Email,
                AvatarUrl = m.AvatarUrl,
                UserIsActive = m.UserIsActive
            })
            .ToList();
            return members;
        }

        private List<AuditLogViewModel> GetAuditLogs(CrewFullProfileResult result)
        {
            var audits = result.CrewRows
            .Where(r => r.AuditLogId.HasValue)
            .Select(r => new AuditLogViewModel
            {
                Id = r.AuditLogId ?? Guid.Empty,
                EntityName = r.AuditLogEntityName!,
                EntityId = r.AuditLogEntityId!,
                Action = r.AuditLogAction!,
                FieldName = r.AuditLogFieldName,
                OldValue = r.AuditLogOldValue,
                NewValue = r.AuditLogNewValue,
                Comment = r.AuditLogComment,
                PerformedByUserId = r.AuditLogPerformedByUserId ?? Guid.Empty,
                PerformedByName = r.AuditLogPerformedByName,
                IpAddress = r.AuditLogIpAddress,
                OrganisationId = r.AuditLogOrganisationId,
                CreatedAt = r.AuditLogCreatedAt ?? DateTime.MinValue,
                ShowEntityType = false,
                ShowComment = r.AuditLogShowComment ?? true,
                VisibleTo = r.AuditLogVisibleTo
            })
            .ToList();
            return audits;
        }
        #endregion
    }
}
