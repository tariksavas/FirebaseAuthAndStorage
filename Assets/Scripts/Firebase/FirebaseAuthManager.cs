namespace Incru.FirebaseFile
{
    using Firebase;
    using Firebase.Auth;
    using Incru.UI;
    using System;
    using System.Threading.Tasks;
    using TMPro;
    using UnityEngine;
    using Firebase.Extensions;

    public class FirebaseAuthManager : MonoBehaviour
    {
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private GameObject skipPanel;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TimerText messageText;

        public FirebaseAuth Auth { get; set; }
        public FirebaseUser User { get; set; }

        public static FirebaseAuthManager Instance;
        public static event Action<FirebaseUser> LoggedIn;
        public static event Action<FirebaseUser> Registered;
        public static event Action LoggedOut;

        private async void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
                await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    var dependencyResult = task.Result;
                    if (dependencyResult == DependencyStatus.Available)
                    {
                        Auth = FirebaseAuth.DefaultInstance;
                        User = Auth?.CurrentUser;
                    }
                    else
                    {
                        Debug.LogError($"Can not inintalize Firebase packages!!!");
                    }
                });

                skipPanel.SetActive(Auth.CurrentUser != null);
                loginPanel.SetActive(Auth.CurrentUser == null);
            }
        }

        public async void LoginClicked()
        {
            var result = await Login(emailField.text, passwordField.text);
            SetTimerText(result.Item2, result.Item3);

            if (result.Item1)
                LoggedIn?.Invoke(User);
        }

        public async void RegisterClicked()
        {
            var result = await Register(emailField.text, passwordField.text);
            SetTimerText(result.Item2, result.Item3);
            
            if (result.Item1)
                Registered?.Invoke(User);
        }

        public async void ResetPasswordClicked()
        {
            var result = await ResetPassWord(emailField.text);
            SetTimerText(result.Item1, result.Item2);
        }

        public void LoginWithAnotherAuth()
        {
            loginPanel.SetActive(true);
            skipPanel.SetActive(false);
        }

        public void Skip()
        {
            LoggedIn?.Invoke(User);
            Debug.Log($"Logged in with {Auth.CurrentUser.UserId}");
        }

        public void LogoutClicked()
        {
            var result = Logout();
        }

        private void SetTimerText(string result1, Color result2)
        {
            messageText.CloseTime = 5f;
            messageText.text = result1;
            messageText.color = result2;
        }

        private async Task<(bool, string, Color)> Login(string email, string password)
        {
            if (Auth == null)
                return (false, "Can not initialize database!", Color.red);

            var message = string.Empty;

            return await Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(loginTask =>
            {
                if (loginTask.Exception == null)
                {
                    User = loginTask.Result;
                    LoggedIn?.Invoke(User);
                    message = "Successfully logged in";
                    return (true, message, Color.green);
                }
                else
                {
                    var exception = loginTask.Exception.GetBaseException() as FirebaseException;
                    message = $"Failed to login - {(AuthError)exception.ErrorCode}";
                    return (false, message, Color.red);
                }
            });
        }

        private async Task<(bool, string, Color)> Register(string email, string password)
        {
            if (Auth == null)
                return (false, "Can not initialize database!", Color.red);

            var message = string.Empty;

            return await Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(registerTask =>
            {
                if (registerTask.Exception == null)
                {
                    message = "Successfully registered";
                    return (true, message, Color.green);
                }
                else
                {
                    var exception = registerTask.Exception.GetBaseException() as FirebaseException;
                    message = $"Failed to register - {(AuthError)exception.ErrorCode}";
                    return (false, message, Color.red);
                }
            });
        }

        private async Task<(string, Color)> ResetPassWord(string email)
        {
            if (Auth == null)
                return ("Can not initialize database!", Color.red);

            var message = string.Empty;

            return await Auth.SendPasswordResetEmailAsync(email).ContinueWith(resetTask =>
            {
                if (resetTask.Exception == null)
                {
                    message = "Email sent";
                    return (message, Color.green);
                }
                else
                {
                    var exception = resetTask.Exception.GetBaseException() as FirebaseException;
                    message = $"Failed to password reset - {(AuthError)exception.ErrorCode}";
                    return (message, Color.red);
                }
            });
        }

        private bool Logout()
        {
            if (Auth == null)
                return false;

            Auth.SignOut();
            LoggedOut?.Invoke();
            return true;
        }
    }
}