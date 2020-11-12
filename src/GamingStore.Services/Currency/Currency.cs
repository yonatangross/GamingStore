namespace GamingStore.Services.Currency
{
    public class Currency
    {
        //[DisplayFormat(DataFormatString = "₪ {0:n}", ApplyFormatInEditMode = true)]
        public string EUR { get; set; }

        //[DisplayFormat(DataFormatString = "$ {0:n}", ApplyFormatInEditMode = true)]
        public string GBP { get; set; }

        //[DisplayFormat(DataFormatString = "€ {0:C}", ApplyFormatInEditMode = true)]
        public string ILS { get; set; }
    }
}
