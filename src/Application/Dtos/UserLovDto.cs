namespace ContractorBackend.Application.Dtos
{
    public class UserLovDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }



        /// <summary>
        /// شماره پرسنلی فرد
        /// </summary>
        public string PersonnelCode { get; set; }
        /// <summary>
        /// نام
        /// </summary> 
        public string? FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary> 
        public string? LastName { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string NationalCode { get; set; }



    }
}
