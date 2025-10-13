using System;
using System.Threading.Tasks;
using ContractorBackend.Application.Models;

namespace ContractorBackend.Application.Common.Services
{
    public class MscAuthenticateService : IMscAuthenticateService
    {
        public MscAuthenticateService()
        {

        }


        public string GetReferrer(string code)
        {
            Nipcrypt createReferer = new Nipcrypt();
            createReferer.nipen(code + DateTime.Now.ToString("yyyyMMddHHmmss"));
            return createReferer.nipenRes.ToString();
        }

        public bool IsTicketValid(string ticket, long personnelCode)
        {
            //
            //TODO: check ticket
            return true;
        }

        public async Task<MscLoginResult> CheckMscLoginAsync(string ticket)
        {
            Nipcrypt _Nipcrypt = new();
            _Nipcrypt.nipde(ticket);
            string _ticket = _Nipcrypt.nipdeRes.ToString();
            DateTime DateTicket = DateTime.ParseExact(_ticket.Substring(_ticket.Length - 14, 14), "yyyyMMddHHmmss", null);
            string tecketPersonal = _ticket[0..^14];
            if (!long.TryParse(tecketPersonal, out long personnelCode))
            {
                return null;
            }

            //  MscService.AuthenticateLoginCheckerSoapClient _WebSrvAuthLogin = new MscService.AuthenticateLoginCheckerSoapClient(MscService.AuthenticateLoginCheckerSoapClient.EndpointConfiguration.AuthenticateLoginCheckerSoap);
            //string tecketPersonal = ticket.Substring(0, ticket.Length - 14);

            //_Nipcrypt.nipen(tecketPersonal + DateTime.Now.ToString("yyyyMMddHHmmss"));
            //_Nipcrypt.nipde((await _WebSrvAuthLogin.CheckerAsync(_Nipcrypt.nipenRes.ToString())).Body.CheckerResult);
            if (!string.IsNullOrEmpty(_Nipcrypt.nipdeRes) && _Nipcrypt.nipdeRes != "0" && long.TryParse(_Nipcrypt.nipdeRes, out long t))
            {
                return new MscLoginResult()
                {
                    PersonnelCode = personnelCode,
                    Ticket = ticket
                };
            }
            return null;
        }


    }
}
