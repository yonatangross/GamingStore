using System.ComponentModel.DataAnnotations;

namespace GamingStore.ViewModels.Errors
{
    public class ErrorViewModel:ViewModelBase
    {
        [DataType(DataType.Text)]
        public string ErrorCode { get; set; }
    }
}