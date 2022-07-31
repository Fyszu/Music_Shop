namespace Music_Shop.Data
{
    public class OAuthResponse
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public int Expires_in { get; set; }
        public string Grant_type { get; set; }
        public DateTime AcquisitionTime { get; set; }
    }
}
