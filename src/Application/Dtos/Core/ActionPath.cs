namespace ContractorBackend.Application.Dtos.Core
{
    public class ActionPath
    {
        public string Ctrl { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string DisplayName { get; set; }
        public bool IsGlobal { get; set; }

        public string FinalPath
        {
            get
            {
                var _finalPath = string.Empty;
                if (!string.IsNullOrWhiteSpace(Area))
                    _finalPath = $"{Area}:{Ctrl}:{Action}";
                else
                    _finalPath = $"{Ctrl}:{Action}";

                return _finalPath;
            }
        }
    }
}
