namespace Incru.FirebaseFile
{
    using Firebase;
    using Firebase.Firestore;
    using UnityEngine;

    public class FirebaseManager
    {
        public static FirebaseApp App { get; set; }

        private static void CreateApp()
        {
            var jsonName = "google-services";
            var configJson = Resources.Load<TextAsset>(jsonName);
            App = FirebaseApp.Create(AppOptions.LoadFromJsonConfig(configJson?.text));
        }

        public static FirebaseFirestore GetFirestoreRef()
        {
            if (App == null)
                CreateApp();

            return FirebaseFirestore.GetInstance(App);
        }
    }
}