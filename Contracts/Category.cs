using System.ComponentModel.DataAnnotations;

namespace GamingStore.Contracts
{
    public enum Category
    {
        [Display(Name = "Gaming Headsets")] GamingHeadsets,
        [Display(Name = "Gaming Chairs")] GamingChairs,
        [Display(Name = "Mouse Pads")] MousePads,
        [Display(Name = "GPUs")] GPUs,
        [Display(Name = "CPUs")] CPUs
    }
}