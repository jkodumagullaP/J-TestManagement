using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

//[assembly : SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true, ControlPrincipal = true)]

namespace Utilities
{
    /// <!-- 
    ///     Code Courtesy Of http://www.purecoding.net
    ///     Permissions : Free for general use
    ///     Keywords: Security, Identity, c#, .Net, WinForms, Aspnet. 
    ///     Filename: Impersonator.cs
    /// -->
    /// <summary>
    /// Impersonator - Simply logs in as another identity. 
    /// Recommended usage:  
    ///     using(Impersonator imp = new Impersonator("domain", "user", password))
    ///     {
    ///         //Do stuff here
    ///     }//Automatically reverts on dispose.
    /// </summary>
    /// <remarks>
    /// Uses Interactive logon.
    /// Calls unmanaged code.
    /// Uses SecureString for additional protection, alternatively you can pass
    ///  a char array eg password.ToCharArray()
    /// </remarks>
    public sealed class Impersonator : IDisposable
    {
        #region Win32 Unmanaged API Calls and Constants

        private const int LOGON32_LOGON_INTERACTIVE = 2;
        private const int LOGON32_LOGON_NETWORK = 3;
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(string userName, string domain, string password,
                                             int logonType, int logonProvider, ref IntPtr accessToken);

        #endregion

        #region Private variables

        private string _Domain;
        private WindowsImpersonationContext _ImpersonationContext;

        [NonSerialized] private SecureString _Password;

        private string _UserName;

        #endregion

        #region Constructors Destructor

        public Impersonator()
        {
            string userName = ConfigurationManager.AppSettings["RestrictedDomainUserName"];
            if (ConfigurationManager.AppSettings["RestrictedDomainPassword"] != null)
            {
                char[] password =
                    ConfigurationManager.AppSettings["RestrictedDomainPassword"].ToCharArray();
                Setup(GetDomain(userName), GetUserName(userName), GetPassword(ref password));
            }
            else
            {
                throw new ConfigurationErrorsException(
                    "Unable to launch default impersonator as key RestrictedDomainPassword" +
                    " does not have a value in the configuration.");
            }
        }

        public Impersonator(string userName, char[] password)
        {
            Setup(GetDomain(userName), GetUserName(userName), GetPassword(ref password));
        }

        public Impersonator(string userName, SecureString password)
        {
            Setup(GetDomain(userName), GetUserName(userName), password);
        }

        public Impersonator(string domain, string userName, char[] password)
        {
            Setup(GetDomain(domain), GetUserName(userName), GetPassword(ref password));
        }

        public Impersonator(string domain, string userName, SecureString password)
        {
            Setup(GetDomain(domain), GetUserName(userName), password);
        }

        ~Impersonator()
        {
            Revert(); //Incase someone forgets to dispose.
        }

        private void Setup(string domain, string userName, SecureString password)
        {
            if (string.IsNullOrEmpty(userName) || password == null)
            {
                throw new ArgumentException(
                    "Username or Password cannot be null in Impersonator constructor. " + 
                    " Username must not be an empty string.");
            }
            _UserName = userName;
            _Domain = domain;
            _Password = password;
            _ImpersonationContext = Impersonate();
        }

        #endregion

        #region IDisposable Members

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        ///</summary>
        ///<filterpriority>2</filterpriority>
        public void Dispose()
        {
            Revert();
        }

        #endregion

        private WindowsImpersonationContext Impersonate()
        {
            IntPtr accessToken = IntPtr.Zero;
            IntPtr passwordPtr = Marshal.SecureStringToBSTR(_Password);
            try
            {
                bool success =
                    LogonUser(_UserName, _Domain, Marshal.PtrToStringUni(passwordPtr),
                              LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref accessToken);
                if (!success)
                {
                    //Error
                    throw new ApplicationException(
                        string.Format(
                            "Impersonation failed as user {0} with Win32 Error Number: {1}. " + 
                            "  Most likely the username or password is invalid.",
                            _UserName,
                            Marshal.GetLastWin32Error()));
                }
                WindowsIdentity identity = new WindowsIdentity(accessToken);
                return identity.Impersonate();
            }
            finally
            {
                Marshal.ZeroFreeBSTR(passwordPtr); //Clear the password for further security.
            }
        }

        public void Revert()
        {
            if (_ImpersonationContext != null)
            {
                _ImpersonationContext.Undo();
                _ImpersonationContext.Dispose();
                _ImpersonationContext = null;
            }
        }

        #region Helper methods for dealing with parameters.

        private static SecureString GetPassword(ref char[] password)
        {
            SecureString securePassword = new SecureString();
            for (int count = 0; count < password.Length; count++)
            {
                securePassword.AppendChar(password[count]);
            }
            Array.Clear(password, 0, password.Length);
            securePassword.MakeReadOnly();
            return securePassword;
        }

        private static string GetUserName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return string.Empty;
            }
            if (!userName.Contains("\\"))
            {
                return userName;
            }
            else
            {
                int seperator = userName.IndexOf("\\");
                return userName.Substring(seperator + 1);
            }
        }

        private static string GetDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return string.Empty;
            }
            if (!domain.Contains("\\"))
            {
                return domain;
            }
            else
            {
                int seperator = domain.IndexOf("\\");
                return domain.Substring(0, seperator);
            }
        }

        #endregion
    }
}