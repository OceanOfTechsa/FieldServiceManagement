using FieldServiceManagement.Business.MappingBusiness;
using FieldServiceManagement.Data.DataModels.Shared;
using FieldServiceManagement.Enum;
using FieldServiceManagement.Repository.Repositories;
using FieldServiceManagement.ViewModels.Extensions;
using FieldServiceManagement.ViewModels.SubscriptionPlan;
using FieldServiceManagement.ViewModels.User;
using FieldServiceManagement.Business.Configuration;

namespace FieldServiceManagement.Business
{
    public class SubscriptionPlanBusiness
    {
        public async Task<SubscriptionPlanViewModel> GetSubscriptionPlanByIdAsync(int Id)
        {
            var plan = await new SubscriptionPlanRepository().GetSubscriptionPlanByIdAsync(Id);
            return ObjectMapper.Mapper.Map<SubscriptionPlanViewModel>(plan);
        }

        public async Task<BusinessResult> ApplySubscriptionPlanRules(AppUserProfileViewModel Model,SubscriptionRuleContext Context)
        {

            if (Model?.OrgSubscription?.EndDate < DateTime.Now.SaDateTime())
                return BusinessResult.Fail($"Your subscription expired on {Model?.OrgSubscription?.EndDate:MMM dd, yyyy}. Access to this feature has been restricted. Please renew your subscription or contact support to restore full access.");

            var plan = Model?.SubscriptionPlan;
            if (plan == null)
                return BusinessResult.Fail("No subscription plan found for the current organisation.");

            if (!plan.IsActive)
                return BusinessResult.Fail("Your current subscription plan is inactive. Please contact support or upgrade your plan.");


            var orgId = Model!.User.OrganisationId;

            return Context switch
            {
                SubscriptionRuleContext.AddUser => await CheckMaxUsers(plan, orgId),
                SubscriptionRuleContext.CreateCrew => await ShouldAllowCrewCreation(plan),
                //SubscriptionRuleContext.AddWorkOrder => await CheckMaxWorkOrders(plan, orgId),
                //SubscriptionRuleContext.AddForm => await CheckMaxForms(plan, orgId),
                //SubscriptionRuleContext.AddStorage => await CheckMaxStorage(plan, orgId),
                _ => BusinessResult.Ok()
            };
        }


        #region PRIVATE METHODS

        private async Task<BusinessResult> CheckMaxUsers(SubscriptionPlanViewModel plan, Guid orgId)
        {
            if (!plan.MaxUsers.HasValue) return BusinessResult.Ok();

            var count = await new UserRepository().GetNumberOfUsersByOrganisationId(orgId);
            if (count >= plan.MaxUsers.Value)
                return BusinessResult.Fail($"Your plan allows a maximum of {plan.MaxUsers.Value} user(s). " +
                                           $"Please upgrade your plan to add more users.");
            return BusinessResult.Ok();
        }

        private async Task<BusinessResult> ShouldAllowCrewCreation(SubscriptionPlanViewModel Plan)
        {
            if(Plan.Id == (int)SubscriptionPlanEnum.Free || Plan.Id == (int)SubscriptionPlanEnum.Starter)
                return BusinessResult.Fail("Sorry, your current edition of " + AppSettings.AppName + " does not support the Crew feature.\r\nUpgrade to continue.");
            
            return BusinessResult.Ok();
        }

        //private async Task<BusinessResult> CheckMaxWorkOrders(SubscriptionPlanViewModel plan, Guid orgId)
        //{
        //    if (!plan.MaxWorkOrders.HasValue) return BusinessResult.Ok();

        //    var count = await new WorkOrderRepository().GetNumberOfWorkOrdersByOrganisationId(orgId);
        //    if (count >= plan.MaxWorkOrders.Value)
        //        return BusinessResult.Fail($"Your plan allows a maximum of {plan.MaxWorkOrders.Value} work order(s). " +
        //                                   $"Please upgrade your plan to create more work orders.");
        //    return BusinessResult.Ok();
        //}

        //private async Task<BusinessResult> CheckMaxForms(SubscriptionPlanViewModel plan, Guid orgId)
        //{
        //    if (!plan.MaxForms.HasValue) return BusinessResult.Ok();

        //    var count = await new FormRepository().GetNumberOfFormsByOrganisationId(orgId);
        //    if (count >= plan.MaxForms.Value)
        //        return BusinessResult.Fail($"Your plan allows a maximum of {plan.MaxForms.Value} form(s). " +
        //                                   $"Please upgrade your plan to create more forms.");
        //    return BusinessResult.Ok();
        //}

        //private async Task<BusinessResult> CheckMaxStorage(SubscriptionPlanViewModel plan, Guid orgId)
        //{
        //    if (!plan.MaxStorageMb.HasValue) return BusinessResult.Ok();

        //    var usedMb = await new StorageRepository().GetUsedStorageMbByOrganisationId(orgId);
        //    if (usedMb >= plan.MaxStorageMb.Value)
        //        return BusinessResult.Fail($"Your plan allows a maximum of {plan.MaxStorageMb.Value} MB of storage. " +
        //                                   $"Please upgrade your plan or free up space.");
        //    return BusinessResult.Ok();
        //}
        #endregion
    }
}
