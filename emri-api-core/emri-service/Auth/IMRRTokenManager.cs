using static emri_service.Auth.MRRTokenManager;

namespace emri_service.Auth
{
	public interface IMRRTokenManager
	{
		public TokenDetails GetTokenDetails(string token);
		public bool IsValidToken(string base64token, out TokenDetails details);
		public string GetMRRToken();
	}
}
