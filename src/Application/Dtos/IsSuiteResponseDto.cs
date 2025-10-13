namespace ContractorBackend.Application.Dtos.scs
{
    public class IsSuiteResponseDto
    {
        /// <summary>
        /// یک عملیات موفق - غیر یک عملیات نا موفق که متن خطا در پارامترمربوطه قرار گرفته است
        /// </summary>
        public int lv_res { get; set; }

        /// <summary>
        ///  متن خطا
        /// </summary>
        public string p_errmsg { get; set; }

    }
    public class IsSuiteResponse2Dto
    {
        public int lv_res { get; set; }

        /// <summary>
        ///  متن خطا
        /// </summary>
        public string p_errmsg { get; set; }

    }

    public class IsSuiteResponseIntDto
    {
        /// <summary>
        /// یک عملیات موفق - غیر یک عملیات نا موفق که متن خطا در پارامترمربوطه قرار گرفته است
        /// </summary>
        public int lv_res { get; set; }

        /// <summary>
        ///  متن خطا
        /// </summary>
        public string p_errmsg { get; set; }

        public long? P_NEW_INFO_BANK_ID { get; set; }

    }
}
