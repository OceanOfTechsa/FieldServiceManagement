namespace FieldServiceManagement.Enum
{
    public enum AuditEventType
    {
        // CRUD & General
        Created, Updated, Deleted, Viewed, Restored, Archived, Duplicated,
        // User & Access
        Invited, Removed, Shared, Locked, Unlocked, RoleChanged, PermissionGranted, PermissionRevoked,
        // Communication
        Commented, Mentioned, Notified, EmailSent, SmsSent,
        // Files & Documents
        Downloaded, Uploaded, Signed, Expired,
        // Status & Workflow
        Approved, Rejected, Submitted, Assigned, Unassigned, Escalated, Resolved, Cancelled, Completed, Pending,
        // Tagging & Classification
        Starred, Tagged, Flagged, Prioritized,
        // Settings & Config
        SettingsChanged, Linked,
        // Vehicle Operations
        VehicleAdded, VehicleRemoved, VehicleActivated, VehicleDeactivated,
        IgnitionOn, IgnitionOff, EngineStarted, EngineStopped, OdometerUpdated,
        // Trip & Route
        TripStarted, TripEnded, TripPaused, TripResumed,
        RouteAssigned, RouteCompleted, RouteDeviated, EtaUpdated,
        // Location & Geofence
        LocationUpdated, GeofenceEntered, GeofenceExited, GeofenceCreated, GeofenceDeleted,
        // Driver Management
        DriverAssigned, DriverUnassigned, DriverCheckedIn, DriverCheckedOut,
        LicenseExpiring, LicenseExpired, LicenseRenewed,
        // Maintenance & Service
        MaintenanceScheduled, MaintenanceStarted, MaintenanceCompleted, MaintenanceOverdue,
        ServiceReminder, InspectionPassed, InspectionFailed, InspectionScheduled,
        PartReplaced, RecallIssued,
        // Fuel & Costs
        FuelAdded, FuelTheftDetected, ExpenseLogged, ExpenseApproved, ExpenseRejected,
        // Alerts & Compliance
        SpeedingDetected, HarshBraking, HarshAcceleration, IdleAlert, FatigueAlert,
        ComplianceViolation, ComplianceCleared, InsuranceExpiring, InsuranceRenewed,
        RegistrationExpiring, RegistrationRenewed,
        // Cargo & Delivery
        CargoLoaded, CargoUnloaded, DeliveryAttempted, DeliveryCompleted, DeliveryFailed, PickupCompleted,
        // Telematics & Diagnostics
        DtcDetected, DtcCleared, BatteryLow, TemperatureAlert, DeviceConnected, DeviceDisconnected,
        // FSM Platform Actions
        UserCreated, UserInvited, UserDeleted, UserUpdated, UserProfileAccessed,
        UserLockedOut, UserUnblocked,
        WorkOrderSubmitted, WorkOrderAssigned, WorkOrderCompleted, WorkOrderStatusChanged,
        InvoiceGenerated, InvoiceStatusChanged,
        ServiceAppointmentScheduled, ServiceAppointmentRescheduled, Inbox
    }
}
