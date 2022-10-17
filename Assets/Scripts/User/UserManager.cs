namespace Incru.User
{
    using Firebase.Auth;
    using Firebase.Extensions;
    using Firebase.Firestore;
    using Incru.FirebaseFile;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using Incru.DataModels;
    using Incru.Structure;
    using Incru.UI;

    public class UserManager : MonoBehaviour
    {
        public UserInformation UserInformation { get; set; }
        public MyList<CharacterInformation> CharacterInformations { get; set; }

        public static event Action Loaded;

        public static UserManager Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            FirebaseAuthManager.LoggedIn += OnLoggedIn;
        }

        private void OnDestroy()
        {
            FirebaseAuthManager.LoggedIn -= OnLoggedIn;
        }

        private async void OnLoggedIn(FirebaseUser user)
        {
            LoadingLoop.Instance.StackedLoaders.Add("logged-in-db-loader");

            var firestoreRef = FirebaseManager.GetFirestoreRef();
            var documentReference = firestoreRef.Collection($"Users").Document(user.UserId);
            await documentReference.GetSnapshotAsync().ContinueWithOnMainThread(task =>
            {
                try
                {
                    if (task.IsCompleted)
                    {
                        var snapshot = task.Result;
                        if (snapshot.Exists)
                        {
                            UserInformation = snapshot.ConvertTo<UserInformation>();
                            FillCharacterInformations(task =>
                            {
                                Loaded?.Invoke();
                                LoadingLoop.Instance.StackedLoaders.Remove("logged-in-db-loader");
                            });
                            return;
                        }
                        LoadingLoop.Instance.StackedLoaders.Remove("logged-in-db-loader");
                        UIController.Instance.InitPanel(true);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e}");
                    LoadingLoop.Instance.StackedLoaders.Remove("logged-in-db-loader");
                }
            });
        }

        public async Task SetUserDataFirstTime(UserInformation userInformation, List<CharacterInformation> characterInformations)
        {
            var user = FirebaseAuthManager.Instance.User;
            var firestoreRef = FirebaseManager.GetFirestoreRef();

            WriteBatch writeBatch = firestoreRef.StartBatch();

            UserInformation = userInformation;
            var userDocumentRef = firestoreRef.Collection($"Users").Document(user.UserId);
            writeBatch.Set(userDocumentRef, UserInformation, SetOptions.MergeAll);

            foreach (var characterInformation in characterInformations)
            {
                var characterInformationDocumentRef = userDocumentRef.Collection("CharacterInformations").Document();
                writeBatch.Set(characterInformationDocumentRef, characterInformation, SetOptions.MergeAll);
            }

            await writeBatch.CommitAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Added to db this user - {user.UserId}");
                }
                else
                {
                    Debug.LogError($"Can not add to db this user - {user.UserId}");
                }
            });
            Loaded?.Invoke();
        }

        private async void FillCharacterInformations(Action<bool> filled)
        {
            CharacterInformations = new MyList<CharacterInformation>();

            var firestoreRef = FirebaseManager.GetFirestoreRef();
            var collectionReference = firestoreRef.Collection($"Users").Document(FirebaseAuthManager.Instance.User.UserId).Collection("CharacterInformations");
            await collectionReference.GetSnapshotAsync().ContinueWith(task =>
            {
                try
                {
                    if (task.IsCompleted)
                    {
                        var documents = task.Result.Documents;
                        foreach (var document in documents)
                        {
                            var characterInformation = document.ConvertTo<CharacterInformation>();
                            CharacterInformations.Add(characterInformation);
                            return;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{e}");
                    filled?.Invoke(false);
                }
            });
            filled?.Invoke(true);
        }
    }
}