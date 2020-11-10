using System.ComponentModel.DataAnnotations;

namespace GamingStore.ViewModels
{
    public class ErrorViewModel:ViewModelBase
    {
        [DataType(DataType.Text)]
        public string ErrorCode { get; set; }
    }
}