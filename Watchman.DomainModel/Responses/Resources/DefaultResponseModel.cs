namespace Watchman.DomainModel.Responses.Areas.Administration
{
    public class DefaultResponseModel
    {
        public string OnEvent { get; set; }
        public string Value { get; set; }
        public string[] Variables { get; set; }
        public bool RequireContext { get; set; }
    }
}
