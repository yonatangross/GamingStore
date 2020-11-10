using System;
using System.ComponentModel.DataAnnotations;

namespace GamingStore.Models
{
    public class ErrorViewModel
    {
        [DataType(DataType.Text)]
        public string ErrorCode { get; set; }
    }
}