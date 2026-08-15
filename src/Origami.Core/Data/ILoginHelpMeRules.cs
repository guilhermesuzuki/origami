using Origami.Core.Models;

namespace Origami.Core.Data
{
    public interface ILoginHelpMeRules
    {
        event EventHandler CurrentStepChanged;
        event EventHandler GoToLoginPage;

        event EventHandler RefreshUI;
        enum Steps
        {
            Step1_ValidateMasterPassword = 1,
            Step2_CreateNewAdminUser = 2,
            Step3_GoToLoginPage = 3,
        }

        /// <summary>
        /// New admin user to be created in the system after the master password has been validated successfully.
        /// </summary>
        OrigamiUser NewAdminUser { get; set; }

        /// <summary>
        /// 1-time master password for verification. This password is used to validate the user's identity before allowing them to create a new admin user in the system.
        /// </summary>
        string OneTimeMasterPasswordForVerification { get; set; }

        /// <summary>
        /// Roles to be assigned to the new admin user. This list contains the roles that will be associated with the newly created admin user, defining their permissions and access levels within the system.
        /// </summary>
        IList<OrigamiUserRole> RolesForTheNewAdminUser { get; }

        bool ShouldDisableMasterPasswordVerification { get; }

        Stack<Steps> State { get; }

        /// <summary>
        /// Creates a new admin user in the system. This step is typically executed after the master password has been validated successfully.
        /// </summary>
        /// <returns></returns>
        Task CreateNewAdminUser();

        /// <summary>
        /// Gets a clean instance of the OrigamiUser class. This method returns a new instance of the OrigamiUser class with default values, which can be used as a template for creating new users or resetting user data.
        /// </summary>
        /// <returns></returns>
        OrigamiUser GetCleanUser();

        /// <summary>
        /// Gets the current step in the login help process. This method returns the current step that the user is on, allowing the application to determine what action to take next based on the user's progress through the steps.
        /// </summary>
        /// <returns></returns>
        Steps GetCurrentStep();

        Task GoBackAsync();

        Task GoNextAsync();

        /// <summary>
        /// Everything seems to be okay, so the application should redirect the user to the login page.
        /// </summary>
        /// <returns></returns>
        Task RedirectUserToLoginPage();

        /// <summary>
        /// Validates the master password provided by the user. If the password is valid, it will proceed to the next step; otherwise, it will throw an exception or handle the error accordingly.
        /// </summary>
        /// <returns></returns>
        Task ValidateMasterPassword();
    }
}
