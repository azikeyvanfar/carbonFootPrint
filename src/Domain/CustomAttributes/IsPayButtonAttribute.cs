using System;

namespace ContractorBackend.Domain.CustomAttributes
{
    public class IsPayButton : Attribute
    {
        public bool _isPay { get; set; }
        public IsPayButton(bool isPay)
        {
            _isPay = isPay;
        }
    }
}
