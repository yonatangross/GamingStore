namespace GamingStore.ViewModels.Administration
{
    public class UserRoleViewModel : ViewModelBase
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsSelected { get; set; }
    }
}
