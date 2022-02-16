namespace BlazorServerDemo
{
    public static class ClaimsPrincipalExtentions
    {
        public static string GetLoggedInUserEmail(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            string userEmail = principal.FindFirstValue(ClaimTypes.Email);
            return userEmail;
        }

        //public static IdentityUser<T> GetLoggedInUser(this ClaimsPrincipal principal, SignInManager<T> signInManager)
        //{
        //    if (principal == null)
        //        throw new ArgumentNullException(nameof(principal));

        //    string userEmail = principal.FindFirstValue(ClaimTypes.Email);

        //    return userEmail;
        //}
    }
}