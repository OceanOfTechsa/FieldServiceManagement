using System.ComponentModel.DataAnnotations;

namespace FieldServiceManagement.ViewModels.Crew;

/// <summary>
/// Request body for POST /Workforce/Crew/ChangeLead.
/// Sent as JSON from CrewFullProfile.cshtml's "Change Lead" form.
/// </summary>
public class ChangeCrewLeadRequest
{
    [Required]
    public Guid CrewId { get; set; }

    [Required]
    public Guid UserId { get; set; }
}

/// <summary>
/// Request body for POST /Workforce/Crew/Delete.
/// Sent as JSON from CrewFullProfile.cshtml's "Delete Crew" confirmation dialog.
/// </summary>
public class DeleteCrewRequest
{
    [Required]
    public Guid CrewId { get; set; }
}

/// <summary>
/// Request body for POST /Workforce/Crew/AddMember.
/// Sent as JSON from CrewFullProfile.cshtml's "Add Member to Crew" dialog.
/// </summary>
public class AddCrewMemberRequest
{
    [Required]
    public Guid CrewId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    public bool IsLead { get; set; }
}

/// <summary>
/// Request body for POST /Workforce/Crew/RemoveMember.
/// Sent as JSON from CrewFullProfile.cshtml's member-removal confirmation dialog.
/// </summary>
public class RemoveCrewMemberRequest
{
    [Required]
    public Guid CrewId { get; set; }

    [Required]
    public Guid UserId { get; set; }
}